using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    public class Territory
    {
        [StringLength(20)]
        public string TerritoryID { get; set; }

        [Required, StringLength(50)]
        public string TerritoryDescription { get; set; }

        [Required, ForeignKey("RegionID")]
        public Region Region { get; set; }
    }
}