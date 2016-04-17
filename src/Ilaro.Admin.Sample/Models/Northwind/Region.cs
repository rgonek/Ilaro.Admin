using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Table("Region")]
    public class Region
    {
        public int RegionID { get; set; }

        [Required, StringLength(50)]
        public string RegionDescription { get; set; }
    }
}