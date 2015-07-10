using System.Web.Mvc;
using Ilaro.Admin.Areas.IlaroAdmin.Controllers;

namespace Ilaro.Admin.Infrastructure
{
    public class ClearEntitiesPropertiesAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.IsChildAction == false &&
                (filterContext.Controller.GetType() == typeof(EntitiesController) ||
                filterContext.Controller.GetType() == typeof(EntityController)))
            {
                foreach (var entity in Admin.EntitiesTypes)
                {
                    entity.ClearPropertiesValues();
                }
            }
        }
    }
}