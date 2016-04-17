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
            Entity<Customer>.Register().ReadAttributes();
            Entity<Employee>.Register().ReadAttributes();
            Entity<Order>.Register().ReadAttributes();
            Entity<OrderDetail>.Register().ReadAttributes();
            Entity<Product>.Register().ReadAttributes();
            Entity<Category>.Register().ReadAttributes();
            Entity<EmployeeTerritory>.Register().ReadAttributes();
            Entity<Region>.Register().ReadAttributes();
            Entity<Shipper>.Register().ReadAttributes();
            Entity<Supplier>.Register().ReadAttributes();
            Entity<Territory>.Register().ReadAttributes();
            Entity<EntityChange>.Register().ReadAttributes();

            //Admin.AssemblyCustomizers(typeof(Customer).Assembly).Register();

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