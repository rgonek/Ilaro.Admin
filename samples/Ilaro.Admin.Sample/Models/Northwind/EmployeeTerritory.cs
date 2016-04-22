using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Table("EmployeeTerritories")]
    public class EmployeeTerritory
    {
        [Key]
        [Required, ForeignKey("Employee")]
        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }

        [Key]
        [Required, ForeignKey("TerritoryID")]
        public Territory Territory { get; set; }
    }
}