using System.Collections.Generic;

namespace Ilaro.Admin.Sample.Northwind.Models
{
    public class Shipper
    {
        public int ShipperID { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}