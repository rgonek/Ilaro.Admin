using System;
using System.Web.Mvc;

namespace Ilaro.Admin.Infrastructure
{
    public class ClearEntitiesPropertiesAttribute : IlaroAdminFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (IsIlaroAdminController(filterContext.Controller) == false)
                return;

            foreach (var entity in Admin.Current.Entities)
            {
                entity.ClearPropertiesValues();
            }
        }
    }
}