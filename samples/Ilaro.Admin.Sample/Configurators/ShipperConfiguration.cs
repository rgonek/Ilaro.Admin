using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class ShipperConfiguration : EntityConfiguration<Shipper>
    {
        public ShipperConfiguration()
        {
            Group("Supplier");

            Property(x => x.CompanyName, x =>
            {
                x.Required();
                x.StringLength(40);
            });
            Property(x => x.Phone, x => x.StringLength(24));

            Property(x => x.Orders, x => x.Cascade(CascadeOption.Delete));
        }
    }
}