using System.Collections.Generic;

namespace Ilaro.Admin.Sample.Northwind.Models
{
    public class Territory
    {
        public string TerritoryID { get; set; }

        public string TerritoryDescription { get; set; }

        public Region Region { get; set; }

        public ICollection<EmployeeTerritory> EmployeeTerritories { get; set; }
    }
}