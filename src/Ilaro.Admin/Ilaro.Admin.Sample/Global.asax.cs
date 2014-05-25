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
			AdminInitialise.RegisterResourceRoutes(RouteTable.Routes);
			AdminInitialise.RegisterRoutes(RouteTable.Routes, prefix: "Admin");

			AdminInitialise.AddEntity<Customer>();
			AdminInitialise.AddEntity<Employee>();
			AdminInitialise.AddEntity<Order>();
			AdminInitialise.AddEntity<OrderDetail>();
			AdminInitialise.AddEntity<Product>();
			AdminInitialise.AddEntity<Category>();
			AdminInitialise.AddEntity<EmployeeTerritory>();
			AdminInitialise.AddEntity<Region>();
			AdminInitialise.AddEntity<Shipper>();
			AdminInitialise.AddEntity<Supplier>();
			AdminInitialise.AddEntity<Territory>();
			AdminInitialise.AddEntity<EntityChange>();

			// If you want anonymous access to Ilaro.Admin, skip this line
			// off course you can set Roles and Users for AuthorizeAttribute
			AdminInitialise.Authorize = new System.Web.Mvc.AuthorizeAttribute();

			// If you have only one connection string there is no need to specify it
			AdminInitialise.Initialise("NorthwindEntities");

			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);
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