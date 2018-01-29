using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Demo.Application.ApplicationServices;
using Demo.Application.QueryServices;
using Demo.DTO.ViewModels.Users;
using Microsoft.AspNet.Identity;

namespace Demo.IdentityCore
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public const string ClaimsUserKey = "accountId";
        private readonly UserAppService _userAppService;
        private readonly UserQueryService _userQueryService;

        public ApplicationUserManager(UserStore userStore,
                                      UserQueryService userQueryService,
                                      UserAppService userAppService)
            : base(userStore)
        {
            _userQueryService = userQueryService;
            _userAppService = userAppService;
        }


        public async Task<ApplicationUser> GetUserByPrincipl(IPrincipal user)
        {
            ApplicationUser appUser = null;
            var accountId = (user.Identity as ClaimsIdentity)?.Claims
                                                             .FirstOrDefault(c => c.Type == ClaimsUserKey)
                                                             ?.Value;
            if (!string.IsNullOrWhiteSpace(accountId))
            {
                appUser = await _userQueryService.FindUserByAccountIdAsync(accountId)
                                                 .ConfigureAwait(false);
            }
            return appUser;
        }

        public Task<ApplicationUser> ValidateUserLoginAsync(string userName, string password)
        {
            return _userQueryService.ValidateUserLoginAsync(userName, password);
        }
    }
}