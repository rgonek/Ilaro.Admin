using System.Web.Mvc;

namespace Ilaro.Admin.Areas.IlaroAdmin
{
    public class IlaroAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "IlaroAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "IlaroAdmin_default",
                "IlaroAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
