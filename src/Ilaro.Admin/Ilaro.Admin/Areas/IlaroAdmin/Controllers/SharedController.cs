using System;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class SharedController : Controller
    {
        private readonly Notificator _notificator;

        public SharedController(Notificator notificator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");

            _notificator = notificator;
        }

        [ChildActionOnly]
        public virtual ActionResult Messages()
        {
            return PartialView("_Messages", _notificator);
        }
    }
}
