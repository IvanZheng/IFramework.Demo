using Demo.DTO.ViewModels.Users;
using System.Threading.Tasks;
using System.Web.Mvc;
using Demo.IdentityCore;

namespace Demo.Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationUserManager _userManager;

        public HomeController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        private ApplicationUser _currentUser;
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _currentUser ?? (_currentUser = await _userManager.GetUserByPrincipl(User)
                                                                    .ConfigureAwait(false));
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.CurrentUser = (await GetCurrentUserAsync().ConfigureAwait(false)).UserName;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
    }
}