using Ilaro.Admin.Core.Configuration;
using Ilaro.Admin.Sample.Northwind.Models;

namespace Ilaro.Admin.Sample.Northwind.Configurators
{
    public class EmployeeTerritoryConfiguration : EntityConfiguration<EmployeeTerritory>
    {
        public EmployeeTerritoryConfiguration()
        {
            Group("Employee");

            Table("EmployeeTerritories");

            Property(x => x.EmployeeID, x =>
            {
                x.Id();
                //x.Required();
                x.ForeignKey("Employee");
                x.Display("Employee");
            });

            Property(x => x.Territory, x =>
            {
                x.Id();
                //x.Required();
                x.ForeignKey("TerritoryID");
            });
        }
    }
}