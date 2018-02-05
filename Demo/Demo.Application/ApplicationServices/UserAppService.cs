using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Demo.Domain;
using Demo.Domain.Repositories;
using Demo.Domain.Services;
using Demo.DTO.RequestModels.Accounts;
using IFramework.Exceptions;
using IFramework.Infrastructure;
using IFramework.UnitOfWork;

namespace Demo.Application.ApplicationServices
{
    public class UserAppService : ApplicationServiceBase
    {
        private static readonly TimeSpan LockTimeOut = TimeSpan.FromSeconds(30);
        private readonly ILockProvider _lockProvider;
        private readonly UserDomainService _userDomainService;

        public UserAppService(IDemoRepository repository,
                              IAppUnitOfWork unitOfWork,
                              UserDomainService userDomainService,
                              ILockProvider lockProvider)
            : base(repository, unitOfWork)
        {
            _userDomainService = userDomainService;
            _lockProvider = lockProvider;
        }

        public async Task RegisterUserAsync(RegisterUserRequest request)
        {
            await _lockProvider.LockAsync(request.UserName,
                                          async () =>
                                          {
                                              try
                                              {
                                                  var accountId = await Repository.GetNextSequenceAsync("DbSequence")
                                                                                  .ConfigureAwait(false);
                                                  var user = await _userDomainService.RegisterUserAsync(accountId,
                                                                                                        request.UserName,
                                                                                                        request.Password)
                                                                                     .ConfigureAwait(false);
                                                  await CommitAsync(user.GetInfo()).ConfigureAwait(false);
                                              }
                                              catch (Exception e)
                                              {
                                                  if (e.GetBaseException() is SqlException sqlException)
                                                  {
                                                      // Cannot insert duplicate key row in object '%.*ls' with unique index '%.*ls'.
                                                      if (sqlException.Number == 2601)
                                                      {
                                                          throw new DomainException(Error.UserNameAlreadyExists,
                                                                                    new object[] {request.UserName});
                                                      }
                                                  }
                                                  throw;
                                              }
                                          },
                                          LockTimeOut)
                               .ConfigureAwait(false);
        }

        public async Task RegisterUsersBatchAsync(IEnumerable<RegisterUserRequest> requests)
        {
            foreach (var request in requests)
            {
                try
                {
                    var accountId = await Repository.GetNextSequenceAsync("DbSequence")
                                                    .ConfigureAwait(false);
                    _userDomainService.RegisterUser(accountId, request.UserName, request.Password);
                }
                catch (Exception e)
                {
                    if (e.GetBaseException() is SqlException sqlException)
                    {
                        // Cannot insert duplicate key row in object '%.*ls' with unique index '%.*ls'.
                        if (sqlException.Number == 2601)
                        {
                            throw new DomainException(Error.UserNameAlreadyExists,
                                                      new object[] {request.UserName});
                        }
                    }
                    throw;
                }
            }
            await CommitAsync().ConfigureAwait(false);
        }
    }
}