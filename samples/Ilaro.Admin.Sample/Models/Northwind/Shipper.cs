using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Verbose(GroupName = "Supplier")]
    public class Shipper
    {
        public int ShipperID { get; set; }

        [Required, StringLength(40)]
        public string CompanyName { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }

        [Cascade(CascadeOption.Delete)]
        public ICollection<Order> Orders { get; set; }
    }
}