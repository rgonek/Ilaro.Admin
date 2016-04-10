using System.ComponentModel.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    public class Shipper
    {
        public int ShipperID { get; set; }

        [Required, StringLength(40)]
        public string CompanyName { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }
    }
}