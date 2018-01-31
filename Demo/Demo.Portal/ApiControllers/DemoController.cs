using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Demo.Application.ApplicationServices;
using Demo.Application.QueryServices;
using Demo.DTO.RequestModels.Accounts;
using Demo.DTO.ViewModels.Users;
using Demo.IdentityCore;
using IFramework.AspNet;
using IFramework.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Demo.Portal.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/demo")]
    public class DemoController : DemoApiControllerBase
    {
        private readonly UserAppService _appService;
        protected IAuthenticationManager AuthenticationManager => Request.GetOwinContext().Authentication;

        public DemoController(UserAppService appService, ApplicationUserManager userManager)
            : base(userManager)
        {
            _appService = appService;
        }


        [AllowAnonymous]
        [Route("timeout")]
        public Task<ApiResult> Timeout()
        {
            return ProcessAsync(() => Task.Delay(TimeSpan.FromSeconds(1)));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public Task<ApiResult> RegisterUser([FromBody]RegisterUserRequest request)
        {
            return ProcessAsync(() => _appService.RegisterUserAsync(request));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public Task<ApiResult> Login([FromBody]LoginRequest request)
        {
            return ProcessAsync(async () =>
            {
                ApplicationUser applicationUser = await UserManager.ValidateUserLoginAsync(request.UserName, request.Password);
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                var identity = await UserManager.CreateIdentityAsync(applicationUser, DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaim(new Claim(ApplicationUserManager.ClaimsUserKey, applicationUser.AccountId));
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
            });
        }
       

        [HttpPost]
        [Route("LoginOut")]
        public ApiResult LoginOut()
        {
            return Process(() => AuthenticationManager.SignOut());
        }
    }
}