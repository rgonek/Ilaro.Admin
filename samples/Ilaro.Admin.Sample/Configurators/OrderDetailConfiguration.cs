using Ilaro.Admin.Configuration;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class OrderDetailConfiguration : EntityConfiguration<OrderDetail>
    {
        public OrderDetailConfiguration()
        {
            Table("Order Details");

            Id(x => x.OrderID, x => x.ProductID);

            Property(x => x.OrderID, x => x.ForeignKey("Order"));
            Property(x => x.UnitPrice, x => x.Required());
            Property(x => x.Quantity, x => x.Required());
            Property(x => x.Discount, x => x.Required());
            Property(x => x.Product, x => x.ForeignKey("ProductID"));
        }
    }
}