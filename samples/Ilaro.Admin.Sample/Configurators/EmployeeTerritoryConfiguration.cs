using Ilaro.Admin.Core.Customization;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
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