using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Infrastructure
{
    public class ModelStateErrorsBuilderAttribute : IlaroAdminFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (IsIlaroAdminController(filterContext.Controller) == false)
                return;

            var notificator = DependencyResolver.Current.GetService<Notificator>();
            var modelState = filterContext.Controller.ViewData.ModelState;
            foreach (var error in notificator.GetModelErrors())
            {
                modelState.AddModelError(error.Key, error.Value);
            }
            notificator.ClearModelErrors();
        }
    }
}