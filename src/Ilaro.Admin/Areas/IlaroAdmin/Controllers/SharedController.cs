using System;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class SharedController : Controller
    {
        private readonly Notificator _notificator;
        private readonly IIlaroAdmin _admin;

        public SharedController(IIlaroAdmin admin, Notificator notificator)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));

            _admin = admin;
            _notificator = notificator;
        }

        [ChildActionOnly]
        public virtual ActionResult Messages()
        {
            return PartialView("_Messages", _notificator);
        }

        [ChildActionOnly]
        public virtual ActionResult UserInfo()
        {
            var autorizationIsEnabled = _admin.Authorize != null;
            return PartialView("_UserInfo", autorizationIsEnabled);
        }
    }
}
