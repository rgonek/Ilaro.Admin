using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Infrastructure
{
    public class NotificatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var view = filterContext.Result as ViewResult;
            if (view == null)
            {
                return;
            }

            var notificator =
                (Notificator)DependencyResolver.Current.GetService(typeof(Notificator));

            var notificator2 = (Notificator)view.TempData["Notificator"];

            foreach (var kvp in notificator2.Messages)
            {
                if (kvp.Value.Count > 0)
                {
                    while (kvp.Value.Count > 0)
                    {
                        notificator.Notificate(kvp.Value.Dequeue(), kvp.Key);
                    }
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var view = filterContext.Result as ViewResult;
            if (view == null)
            {
                return;
            }

            var notificator =
                (Notificator)DependencyResolver.Current.GetService(typeof(Notificator));

            view.TempData["Notificator"] = notificator;
        }
    }
}