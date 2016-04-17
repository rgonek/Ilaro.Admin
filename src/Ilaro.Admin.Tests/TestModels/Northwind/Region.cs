using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Tests.TestModels.Northwind
{
    [Table("Region")]
    public class Region
    {
        public int RegionID { get; set; }

        [Required, MaxLength(50)]
        public string RegionDescription { get; set; }
    }
}