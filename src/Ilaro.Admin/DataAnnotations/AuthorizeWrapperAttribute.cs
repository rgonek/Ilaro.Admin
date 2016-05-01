using System;
using System.Web.Mvc;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizeWrapperAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var authorize = Admin.Current.Authorize;
            if (authorize != null)
            {
                authorize.OnAuthorization(filterContext);
            }
        }
    }
}