using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Sample.Northwind.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public decimal? Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Shipper ShipVia { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}