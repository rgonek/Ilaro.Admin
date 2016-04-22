using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class SupplierConfiguration : EntityConfiguration<Supplier>
    {
        public SupplierConfiguration()
        {
            PropertiesGroup("Main", x => x.CompanyName, x => x.ContactName, 
                x => x.ContactTitle);
            PropertiesGroup("Contact", x => x.Phone, x => x.Fax, x => x.HomePage);
            PropertiesGroup("Address", x => x.Address, x => x.City, x => x.Region, 
                x => x.PostalCode, x => x.Country);

            DisplayProperties(x => x.SupplierID, x => x.CompanyName, 
                x => x.ContactName, x => x.ContactTitle, x => x.Country, x => x.City);

            Property(x => x.CompanyName, x =>
            {
                x.Required();
                x.StringLength(40);
            });
            Property(x => x.ContactName, x =>
            {
                x.StringLength(30);
            });
            Property(x => x.ContactTitle, x =>
            {
                x.StringLength(30);
            });
            Property(x => x.Address, x =>
            {
                x.StringLength(60);
            });
            Property(x => x.City, x =>
            {
                x.StringLength(15);
            });
            Property(x => x.Region, x =>
            {
                x.StringLength(15);
            });
            Property(x => x.PostalCode, x =>
            {
                x.StringLength(10);
            });
            Property(x => x.Country, x =>
            {
                x.StringLength(15);
            });
            Property(x => x.Phone, x =>
            {
                x.StringLength(24);
            });
            Property(x => x.Fax, x =>
            {
                x.StringLength(24);
            });

            Property(x => x.Products, x =>
            {
                x.Cascade(CascadeOption.Delete);
            });
        }
    }
}