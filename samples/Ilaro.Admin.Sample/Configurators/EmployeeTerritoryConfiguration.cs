using Ilaro.Admin.Configuration;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class EmployeeTerritoryConfiguration : EntityConfiguration<EmployeeTerritory>
    {
        public EmployeeTerritoryConfiguration()
        {
            Table("EmployeeTerritories");

            Property(x => x.EmployeeID, x =>
            {
                x.Id();
                x.Required();
                x.ForeignKey("Employee");
                x.Display("Employee");
            });

            Property(x => x.Territory, x =>
            {
                x.Id();
                x.Required();
                x.ForeignKey("TerritoryID");
            });
        }
    }
}