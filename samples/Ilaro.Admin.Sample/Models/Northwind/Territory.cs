using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Verbose(GroupName = "Employee")]
    public class Territory
    {
        [StringLength(20)]
        public string TerritoryID { get; set; }

        [Required, StringLength(50)]
        public string TerritoryDescription { get; set; }

        [Required, ForeignKey("RegionID")]
        public Region Region { get; set; }

        [Cascade(CascadeOption.Delete)]
        public ICollection<EmployeeTerritory> EmployeeTerritories { get; set; }
    }
}