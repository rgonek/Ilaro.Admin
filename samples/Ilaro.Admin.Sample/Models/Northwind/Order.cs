using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    public class Order
    {
        public int OrderID { get; set; }

        [OnCreate(ValueBehavior.Now)]
        [DisplayFormat(DataFormatString = "dd-MM-yyyy hh:mm")]
        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public decimal? Freight { get; set; }

        [StringLength(40)]
        public string ShipName { get; set; }

        [StringLength(60)]
        public string ShipAddress { get; set; }

        [StringLength(15)]
        public string ShipCity { get; set; }

        [StringLength(15)]
        public string ShipRegion { get; set; }

        [StringLength(10)]
        public string ShipPostalCode { get; set; }

        [StringLength(15)]
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