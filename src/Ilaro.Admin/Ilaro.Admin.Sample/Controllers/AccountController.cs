using System;
using System.Web.Mvc;
using System.Web.Security;
using log4net;

namespace Ilaro.Admin.Sample.Controllers
{
    public class AccountController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountController));
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Group", new { area = "IlaroAdmin" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string password)
        {
            // Dumb authorization
            // For demo purpose is perfect :)
            if (login == "admin" && password == "admin")
            {
                FormsAuthentication.SetAuthCookie(login, false);
                return RedirectToAction("Index", "Group", new { area = "IlaroAdmin" });
            }

            ModelState.AddModelError(String.Empty, "Wrong login data");

            return View();
        }
    }
}
