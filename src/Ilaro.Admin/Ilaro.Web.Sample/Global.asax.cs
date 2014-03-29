using Ilaro.Admin;
using Ilaro.Sample.Models.Northwind;
using Ilaro.Sample.Models.Northwind.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ilaro.Web.Sample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            AdminInitialize.RegisterRoutes(RouteTable.Routes);
            AdminInitialize.RegisterResourceRoutes(RouteTable.Routes);

            AdminInitialize.InitEntity<Customer>();
            AdminInitialize.InitEntity<Employee>();
            AdminInitialize.InitEntity<Order>();
            AdminInitialize.InitEntity<OrderDetail>();
            AdminInitialize.InitEntity<Product>();
            // This is lame, I need ef context, but i remove this later
            AdminInitialize.InitContext(new NorthwindContext());

            AdminInitialize.Initialise();
        }
    }
}