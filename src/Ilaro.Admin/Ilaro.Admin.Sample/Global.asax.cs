using Ilaro.Admin;
using Ilaro.Admin.Sample.Models.Northwind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ilaro.Admin.Sample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			AreaRegistration.RegisterAllAreas();

			AdminInitialise.RegisterResourceRoutes(RouteTable.Routes);
			AdminInitialise.RegisterRoutes(RouteTable.Routes, "Admin");

            RegisterRoutes(RouteTable.Routes);

            AdminInitialise.AddEntity<Customer>();
            AdminInitialise.AddEntity<Employee>();
            AdminInitialise.AddEntity<Order>();
            AdminInitialise.AddEntity<OrderDetail>();
			AdminInitialise.AddEntity<Product>();
			AdminInitialise.AddEntity<EntityChange>();

			// If you have only one connection string there is no need to specify it
            AdminInitialise.Initialise("NorthwindEntities");

            AdminInitialise.Authorize = new System.Web.Mvc.AuthorizeAttribute();
        }

		private void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
			);
		}
    }
}