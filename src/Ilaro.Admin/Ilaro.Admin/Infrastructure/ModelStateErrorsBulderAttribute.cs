using System;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Infrastructure
{
    public class ModelStateErrorsBulderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var notificator = 
                (Notificator)DependencyResolver.Current.GetService(typeof(Notificator));
            var modelState = filterContext.Controller.ViewData.ModelState;
            foreach (var error in notificator.GetModelErrors())
            {
                modelState.AddModelError(error.Key, error.Value);
            }
        }
    }
}