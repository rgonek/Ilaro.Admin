using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Tests.TestModels.Northwind
{
	public class OrderDetail
	{
		[Key]
		public int OrderDetailsID { get; set; }

		[ForeignKey("Order")]
		public int OrderID { get; set; }

		public int ProductID { get; set; }

		[Required]
		public decimal UnitPrice { get; set; }

		[Required]
		public short Quantity { get; set; }

		[Required]
		public float Discount { get; set; }

		//[ForeignKey("OrderID")]
		//public Order Orders { get; set; }

		[ForeignKey("ProductID")]
		public virtual Product Product { get; set; }
	}
}