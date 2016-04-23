using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class OrderConfigurator : EntityConfiguration<Order>
    {
        public OrderConfigurator()
        {
            Group("Order");

            Property(x => x.OrderDate, x =>
            {
                x.OnCreate(ValueBehavior.UtcNow);
                x.Format("dd-MM-yyyy hh:mm tt");
            });
            Property(x => x.RequiredDate, x =>
            {
                x.Format("dd-MM-yyyy HH:mm");
            });

            Property(x => x.ShipName, x =>
            {
                x.StringLength(40);
            });

            Property(x => x.ShipAddress, x =>
            {
                x.StringLength(60);
            });

            Property(x => x.ShipCity, x =>
            {
                x.StringLength(15);
            });

            Property(x => x.ShipRegion, x =>
            {
                x.StringLength(15);
            });

            Property(x => x.ShipPostalCode, x =>
            {
                x.StringLength(10);
            });

            Property(x => x.ShipCountry, x =>
            {
                x.StringLength(15);
            });

            Property(x => x.Customer, x =>
            {
                x.ForeignKey("CustomerID");
            });

            Property(x => x.Employee, x =>
            {
                x.ForeignKey("EmployeeID");
            });

            Property(x => x.ShipVia, x =>
            {
                x.ForeignKey("ShipVia");
            });

            Property(x => x.OrderDetails, x =>
            {
                x.Cascade(CascadeOption.Delete);
            });
        }
    }
}