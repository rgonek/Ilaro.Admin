using System.Web.Mvc;

namespace Ilaro.Admin.Infrastructure
{
    public class CopyIsAjaxRequestFromRequestToViewBagAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var view = filterContext.Result as ViewResult;
            if (view != null)
            {
                view.ViewBag.IsAjaxRequest =
                    filterContext.RequestContext.HttpContext.Request.IsAjaxRequest();
            }
        }
    }
}