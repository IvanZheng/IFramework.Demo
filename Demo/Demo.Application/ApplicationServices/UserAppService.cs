using System;
using System.Threading.Tasks;
using Demo.Domain.Repositories;
using Demo.Domain.Services;
using Demo.DTO.RequestModels.Accounts;
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

        public Task RegisterUserAsync(RegisterUserRequest request)
        {
            return _lockProvider.LockAsync(request.UserName,
                                           async () =>
                                           {
                                               var user = await _userDomainService.RegisterUserAsync(request.UserName,
                                                                                                     request.Password)
                                                                                  .ConfigureAwait(false);
                                               await CommitAsync(user.GetInfo()).ConfigureAwait(false);
                                           },
                                           LockTimeOut);
        }
    }
}