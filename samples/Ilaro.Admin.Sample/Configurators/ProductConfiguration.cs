using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class ProductConfiguration : EntityConfiguration<Product>
    {
        public ProductConfiguration()
        {
            Group("Product");

            PropertiesGroup("Main", x => x.ProductName, x => x.Category, x => x.UnitPrice, x => x.QuantityPerUnit);
            PropertiesGroup("Others", x => x.SupplierID, x => x.ReorderLevel, x => x.Discontinued);
            PropertiesGroup("Stocks", x => x.UnitsInStock, x => x.UnitsOnOrder);

            Property(x => x.SupplierID, x =>
            {
                x.ForeignKey("Supplier");
                x.Display("Supplier");
            });
            Property(x => x.Category, x =>
            {
                x.ForeignKey("CategoryID");
            });
            Property(x => x.UnitPrice, x =>
            {
                x.Format("C2");
            });
            Property(x => x.ProductName, x =>
            {
                x.Required();
                x.StringLength(40);
            });
            Property(x => x.QuantityPerUnit, x =>
            {
                x.StringLength(20);
            });
            Property(x => x.Discontinued, x =>
            {
                x.Required();
                x.DefaultFilter(true);
            });
            Property(x => x.OrderDetails, x =>
            {
                x.Cascade(CascadeOption.Delete);
            });
        }
    }
}