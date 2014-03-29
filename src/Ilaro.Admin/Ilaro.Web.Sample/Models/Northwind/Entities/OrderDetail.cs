using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ilaro.Sample.Models.Northwind.Entities
{
	[Table("Order Details")]
	public class OrderDetail
	{
		[Key]
		public int OrderID { get; set; }

		public int ProductID { get; set; }
		public decimal UnitPrice { get; set; }
		public short Quantity { get; set; }
		public float Discount { get; set; }
		public Order Orders { get; set; }

		[ForeignKey("ProductID")]
		public virtual Product Product { get; set; }
	}
}