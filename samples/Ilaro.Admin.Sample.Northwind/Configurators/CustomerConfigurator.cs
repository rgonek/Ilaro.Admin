using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Configuration;
using Ilaro.Admin.Sample.Northwind.Models;

namespace Ilaro.Admin.Sample.Northwind.Configurators
{
    public class CustomerConfigurator : EntityConfiguration<Customer>
    {
        public CustomerConfigurator()
        {
            PropertiesGroup("Main", c => c.CustomerID, c => c.CompanyName);
            PropertiesGroup("Contact", c => c.ContactName, c => c.ContactTitle, c => c.Phone, c => c.Fax);
            PropertiesGroup("Address", true, c => c.Address, c => c.City, c => c.Region, c => c.PostalCode, c => c.Country);

            Property(c => c.CustomerID, c =>
            {
                //c => c.Required()
            });
            Property(c => c.CompanyName, c =>
            {
                //c.StringLength(40);
            });
            Property(c => c.ContactName, c =>
            {
                //c.Required();
                //c.StringLength(30);
            });
            Property(c => c.ContactTitle, c =>
            {
                //c.StringLength(30);
            });
            Property(c => c.Address, c =>
            {
                //c.StringLength(60);
            });
            Property(c => c.City, c =>
            {
                //c.StringLength(15);
            });
            Property(c => c.Region, c =>
            {
                //c.StringLength(15);
            });
            Property(c => c.PostalCode, c =>
            {
                //c.StringLength(10);
            });
            Property(c => c.Country, c =>
            {
                //c.StringLength(15);
            });
            Property(c => c.Phone, c =>
            {
                //c.StringLength(24);
            });
            Property(c => c.Fax, c =>
            {
                //c.StringLength(24);
            });
            Property(c => c.Orders, c =>
            {
                c.Cascade(CascadeOption.Delete);
            });
        }
    }
}