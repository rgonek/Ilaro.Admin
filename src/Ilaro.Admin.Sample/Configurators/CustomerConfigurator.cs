using Ilaro.Admin.Configuration;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class CustomerConfigurator : EntityConfiguration<Customer>
    {
        public CustomerConfigurator()
        {
            //.ConfigureProperty(PropertyOf<Employee>.Configure(x => x.Photo)
            //    .SetFileOptions(NameCreation.Timestamp, 2000, false, "", "")
            //    .SetImageSettings("", 100, 100))
            //.ConfigureProperty(PropertyOf<Employee>.Configure(x => x.PhotoPath)
            //    .SetFileOptions(NameCreation.UserInput, 2000, false, "content/employee", "")
            //    .SetImageSettings("big", 500, 500)
            //    .SetImageSettings("min", 100, 100));

            Table("Customers");
            Display("Klient", "Klienci");
            Id(x => x.CustomerID);

            DisplayProperties(
                x => x.Address,
                x => x.City,
                x => x.Country,
                x => x.CustomerID,
                x => x.CompanyName);

            SearchProperties(x => x.City);

            DisplayFormat("");

            PropertiesGroup("Main section", c => c.CompanyName);
            PropertiesGroup("Contact section", true, c => c.ContactName, c => c.ContactTitle);
            PropertiesGroup("Super", x => x.Address, x => x.City);

            Property(x => x.CompanyName, x =>
              {
                  x.Column("ComapnyName");
                  x.Display("Client", "Clients");
                  x.Template(display: Templates.Display.Html, editor: Templates.Editor.Html);
                  //x.File();
                  x.Id();
                  //x.Image();
                  x.Searchable();
                  x.Type(Core.DataType.Text);
                  x.Visible();
              });
        }
    }
}