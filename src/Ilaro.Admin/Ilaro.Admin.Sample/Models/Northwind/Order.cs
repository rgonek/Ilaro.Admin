using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Sample.Models.Northwind
{
	public class Order
	{
        [Key]
        public int OrderID { get; set; }

		public string CustomerID { get; set; }

		public int? EmployeeID { get; set; }

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

		[ForeignKey("CustomerID")]
		public virtual Customer Customer { get; set; }

		[ForeignKey("EmployeeID")]
		public virtual Employee Employee { get; set; }

		public virtual ICollection<OrderDetail> OrderDetails { get; set; }
	}
}