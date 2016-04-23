using Ilaro.Admin.Configuration;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class OrderDetailConfiguration : EntityConfiguration<OrderDetail>
    {
        public OrderDetailConfiguration()
        {
            Group("Order");

            Table("Order Details");

            Id(x => x.OrderID, x => x.ProductID);

            Property(x => x.OrderID, x => x.ForeignKey("Order"));
            Property(x => x.Product, x => x.ForeignKey("ProductID"));
            Property(x => x.Quantity, x => x.Required());

            Property(x => x.Discount, x => x
                .Required()
                .Format("0%"));

            Property(x => x.UnitPrice, x => x
                .Required()
                .Format("C"));
        }
    }
}