using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Demo.DTO.ViewModels.Users;
using Demo.IdentityCore;
using IFramework.AspNet;

namespace Demo.Portal.ApiControllers
{
    public class DemoApiControllerBase : ApiControllerBase
    {
        public DemoApiControllerBase(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        protected ApplicationUserManager UserManager { get; set; }
        public ApplicationUser CurrentUser { get; protected set; }

        public override async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            CurrentUser = await UserManager.GetUserByPrincipl(User)
                                           .ConfigureAwait(false);
            return await base.ExecuteAsync(controllerContext, cancellationToken);
        }
    }
}