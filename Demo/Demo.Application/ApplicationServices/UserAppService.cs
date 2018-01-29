using System;
using System.Threading.Tasks;
using Demo.Domain;
using Demo.Domain.Models.Users;
using Demo.Domain.Repositories;
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

        public UserAppService(IDemoRepository repository,
                              IAppUnitOfWork unitOfWork,
                              ILockProvider lockProvider)
            : base(repository, unitOfWork)
        {
            _lockProvider = lockProvider;
        }

        public Task RegisterUserAsync(RegisterUserRequest request)
        {
            return _lockProvider.LockAsync(request.UserName,
                                           async () =>
                                           {
                                               var userExists = await Repository.ExistsAsync(new UserNameSpec(request.UserName))
                                                                                .ConfigureAwait(false);
                                               if (userExists)
                                               {
                                                   throw new DomainException(Error.UserNameAlreadyExists, new object[] {request.UserName});
                                               }

                                               var user = new User(request.UserName);
                                               var account = user.CreateAccount(request.Password);
                                               Repository.Add(user);
                                               Repository.Add(account);
                                               await CommitAsync(user.GetInfo()).ConfigureAwait(false);
                                           },
                                           LockTimeOut);
        }



    }
}