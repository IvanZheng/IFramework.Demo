using System.Web.Mvc;

namespace Demo.Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
    }
}