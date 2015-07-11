using System.Web.Mvc;
using Ilaro.Admin.Areas.IlaroAdmin.Controllers;

namespace Ilaro.Admin.Infrastructure
{
    public class IlaroAdminFilterAttribute : ActionFilterAttribute
    {
        protected virtual bool IsIlaroAdminController(ControllerBase controller)
        {
            var currentType = controller.GetType();
            return currentType == typeof (EntitiesController) ||
                   currentType == typeof (EntityController) ||
                   currentType == typeof (AccountController) ||
                   currentType == typeof (GroupController) ||
                   currentType == typeof (ResourceController) ||
                   currentType == typeof (SharedController);
        }
    }
}