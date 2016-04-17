using System.Web.Mvc;

namespace Ilaro.Admin.Infrastructure
{
    public class CopyIsAjaxRequestFromRequestToViewBagAttribute : IlaroAdminFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (IsIlaroAdminController(filterContext.Controller) == false)
                return;

            var view = filterContext.Result as ViewResult;
            if (view != null)
            {
                view.ViewBag.IsAjaxRequest =
                    filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();
            }
        }
    }
}