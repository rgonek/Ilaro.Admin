using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Core.FileUpload;
using Ilaro.Admin.Fluent;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AdminInitialise.RegisterResourceRoutes(RouteTable.Routes);
			AdminInitialise.RegisterRoutes(RouteTable.Routes, prefix: "Admin");

			Entity<Customer>.Add(); //.ConfigureProperty(PropertyOf<Customer>.Configure(c => c.CompanyName).SetDisplayTemplate(Templates.Display.Html).SetEditorTemplate(Templates.Editor.Html))
			//.AddPropertiesGroup("Main section", c => c.CompanyName)
			//.AddPropertiesGroup("Contact section", true, c => c.ContactName, c => c.ContactTitle)
			//.SetKey(x => x.CustomerID)
			//.SetTableName("Customers")
			//.SetColumns(x => x.Address, x => x.City, x => x.Country, x => x.CustomerID, x => x.CompanyName)
			//.SetSearchProperties(x => x.City)
			//.AddPropertiesGroup("Super", x => x.Address, x => x.City)
			//.SetDisplayFormat("")
			//.ConfigureProperty(PropertyOf<Customer>.Configure(x => x.CompanyName));

			Entity<Employee>.Add().SetColumns(x => x.EmployeeID, x => x.LastName, x => x.FirstName, x => x.Title, x => x.BirthDate,
				x => x.HireDate, x => x.Address, x => x.City, x => x.Region, x => x.PostalCode, x => x.PhotoPath, x => x.Photo)
				.ConfigureProperty(PropertyOf<Employee>.Configure(x => x.Photo)
					.SetImageOptions(NameCreation.Timestamp, FileUploadDefault.MaxFileSize, false, FileUploadDefault.ImageExtensions)
					.SetImageSettings("", 100, 100, true, true))
				.ConfigureProperty(PropertyOf<Employee>.Configure(x => x.PhotoPath)
					.SetImageOptions(NameCreation.Timestamp, FileUploadDefault.MaxFileSize, false, FileUploadDefault.ImageExtensions)
					.SetImageSettings("content/employee/min", 100, 100, true, false)
					.SetImageSettings("content/employee/big", 500, 500, false, true));
			Entity<Order>.Add();
			Entity<OrderDetail>.Add().SetTableName("Order Details");
			Entity<Product>.Add();
			Entity<Category>.Add();
			Entity<EmployeeTerritory>.Add();
			Entity<Region>.Add();
			Entity<Shipper>.Add();
			Entity<Supplier>.Add();
			Entity<Territory>.Add();
			Entity<EntityChange>.Add();

			// If you want anonymous access to Ilaro.Admin, skip this line
			// off course you can set Roles and Users for AuthorizeAttribute
			AdminInitialise.Authorize = new AuthorizeAttribute();

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
				defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "Ilaro.Admin.Sample.Controllers" }
			);
		}
	}
}