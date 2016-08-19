using Ilaro.Admin.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core.DataAnnotations;

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