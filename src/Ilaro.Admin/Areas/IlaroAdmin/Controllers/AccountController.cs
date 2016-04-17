using System.Web.Mvc;
using System.Web.Security;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost, ValidateAntiForgeryToken]
        public virtual ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/");
        }
    }
}
