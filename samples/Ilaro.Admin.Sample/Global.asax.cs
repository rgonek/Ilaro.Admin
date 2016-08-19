using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Sample.Models.Northwind;
using System;
using NLog;

namespace Ilaro.Admin.Sample
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Core.Admin.AssemblyCustomizers(typeof(Customer).Assembly).Register();

            // If you want anonymous access to Ilaro.Admin, skip this line
            // off course you can set Roles and Users for AuthorizeAttribute
            // If you have only one connection string there is no need to specify it
            Core.Admin.Initialise("NorthwindEntities", "Admin", new AuthorizeAttribute());

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            GlobalFilters.Filters.Add(new HandleErrorAttribute());
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "Ilaro.Admin.Sample.Controllers" }
            );
        }

        static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Server.ClearError();

            _log.Fatal(exception, "Global error");
        }
    }
}