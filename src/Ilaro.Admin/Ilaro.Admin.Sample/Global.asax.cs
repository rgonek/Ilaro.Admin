using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Sample.Models.Northwind;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Configuration;

namespace Ilaro.Admin.Sample
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Entity<Customer>.Register(); //.ConfigureProperty(PropertyOf<Customer>.Configure(c => c.CompanyName).SetDisplayTemplate(Templates.Display.Html).SetEditorTemplate(Templates.Editor.Html))
            //.AddPropertiesGroup("Main section", c => c.CompanyName)
            //.AddPropertiesGroup("Contact section", true, c => c.ContactName, c => c.ContactTitle)
            //.SetKey(x => x.CustomerID)
            //.SetTableName("Customers")
            //.SetColumns(x => x.Address, x => x.City, x => x.Country, x => x.CustomerID, x => x.CompanyName)
            //.SetSearchProperties(x => x.City)
            //.AddPropertiesGroup("Super", x => x.Address, x => x.City)
            //.SetDisplayFormat("")
            //.ConfigureProperty(PropertyOf<Customer>.Configure(x => x.CompanyName));

            Entity<Employee>.Register()
                .SetColumns(x => x.EmployeeID, x => x.LastName, x => x.FirstName, x => x.Title, x => x.BirthDate,
                    x => x.HireDate, x => x.Address, x => x.City, x => x.Region, x => x.PostalCode, x => x.PhotoPath, x => x.Photo)
                .ConfigureProperty(PropertyOf<Employee>.Configure(x => x.Photo)
                    .SetFileOptions(NameCreation.Timestamp, 2000, false, "", "")
                    .SetImageSettings("", 100, 100))
                .ConfigureProperty(PropertyOf<Employee>.Configure(x => x.PhotoPath)
                    .SetFileOptions(NameCreation.UserInput, 2000, false, "content/employee", "")
                    .SetImageSettings("big", 500, 500)
                    .SetImageSettings("min", 100, 100));
            Entity<Order>.Register();
            Entity<OrderDetail>.Register().SetTableName("Order Details");
            Entity<Product>.Register();
            Entity<Category>.Register();
            Entity<EmployeeTerritory>.Register();
            Entity<Region>.Register();
            Entity<Shipper>.Register();
            Entity<Supplier>.Register();
            Entity<Territory>.Register();
            Entity<EntityChange>.Register();

            // If you want anonymous access to Ilaro.Admin, skip this line
            // off course you can set Roles and Users for AuthorizeAttribute
            // If you have only one connection string there is no need to specify it
            Admin.Initialise("NorthwindEntities", "Admin", new AuthorizeAttribute());

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