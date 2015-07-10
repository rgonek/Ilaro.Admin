using System.Web;

namespace Ilaro.Admin.Core
{
    public class HttpContextUserProvider : IProvidingUser
    {
        public string Current()
        {
            return HttpContext.Current.User.Identity.Name;
        }
    }
}