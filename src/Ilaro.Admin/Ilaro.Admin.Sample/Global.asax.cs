using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Sample.Models.Northwind;
using Ilaro.Admin.Configuration;

namespace Ilaro.Admin.Sample
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //Entity<Customer>.Register();
            Entity<Employee>.Register();
            Entity<Order>.Register();
            Entity<OrderDetail>.Register();
            Entity<Product>.Register();
            Entity<Category>.Register();
            Entity<EmployeeTerritory>.Register();
            Entity<Region>.Register();
            Entity<Shipper>.Register();
            Entity<Supplier>.Register();
            Entity<Territory>.Register();
            Entity<EntityChange>.Register();

            Admin.AssemblyCustomizators(typeof(Customer).Assembly).Register();

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