using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Tests.TestModels.Northwind
{
    public class Order
    {
        public int OrderID { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public decimal? Freight { get; set; }

        [MaxLength(40)]
        public string ShipName { get; set; }

        [MaxLength(60)]
        public string ShipAddress { get; set; }

        [MaxLength(15)]
        public string ShipCity { get; set; }

        [MaxLength(15)]
        public string ShipRegion { get; set; }

        [MaxLength(10)]
        public string ShipPostalCode { get; set; }

        [MaxLength(15)]
        public string ShipCountry { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("ShipVia")]
        public virtual Shipper ShipVia { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}