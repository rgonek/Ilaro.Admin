using System;
using System.Web;

namespace Ilaro.Admin.Core
{
    public class HttpContextUserProvider : IProvidingUser
    {
        public string CurrentUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public object CurrentId()
        {
            throw new NotImplementedException("Default Ilaro.Admin HttpContextUserProvider cannot determine current user id.");
        }
    }
}