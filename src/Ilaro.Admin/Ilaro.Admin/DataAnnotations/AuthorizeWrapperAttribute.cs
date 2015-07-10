using System;
using System.Web.Mvc;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizeWrapperAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Admin.Authorize != null)
            {
                Admin.Authorize.OnAuthorization(filterContext);
            }
        }
    }
}