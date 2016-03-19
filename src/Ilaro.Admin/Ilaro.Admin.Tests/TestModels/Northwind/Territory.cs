using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Tests.TestModels.Northwind
{
    public class Territory
    {
        [MaxLength(20)]
        public string TerritoryID { get; set; }

        [Required, MaxLength(50)]
        public string TerritoryDescription { get; set; }

        [Required, ForeignKey("RegionID")]
        public Region Region { get; set; }
    }
}