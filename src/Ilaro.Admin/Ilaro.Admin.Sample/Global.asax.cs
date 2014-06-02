using Ilaro.Admin;
using Ilaro.Admin.Fluent;
using Ilaro.Admin.Sample.Models.Northwind;
using Ilaro.Admin.ViewModels;
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

			Entity<Customer>.Add()
				//.SetKey(x => x.CustomerID)
				//.SetTableName("Customers")
				//.SetDisplayProperties(x => x.Address, x => x.City, x => x.Country, x => x.CustomerID, x => x.CompanyName)
				//.SetSearchProperties(x => x.City)
				//.AddPropertiesGroup("Super", x => x.Address, x => x.City)
				//.SetDisplayFormat("")
				.ConfigureProperty(Property<Customer>.Configure(x => x.CompanyName).Required());

			//Entity<Employee>.Add().SetDisplayProperties(x => x.EmployeeID, x => x.LastName, x => x.FirstName, x => x.Title, x => x.TitleOfCourtesy, x => x.BirthDate,
			//	x => x.HireDate, x => x.Address, x => x.City, x => x.Region, x => x.PostalCode, x => x.Country);
			//Entity<Order>.Add();
			//Entity<OrderDetail>.Add().SetTableName("Order Details");
			//Entity<Product>.Add();
			//Entity<Category>.Add();
			//Entity<EmployeeTerritory>.Add();
			//Entity<Region>.Add();
			//Entity<Shipper>.Add();
			//Entity<Supplier>.Add();
			//Entity<Territory>.Add();
			//Entity<EntityChange>.Add();

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