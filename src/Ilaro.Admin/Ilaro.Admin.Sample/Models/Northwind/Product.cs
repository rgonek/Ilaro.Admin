using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Sample.Models.Northwind
{
	public class Product
	{
		[Key]
		public int ProductID { get; set; }

		[Required]
		[StringLength(20)]
		public string ProductName { get; set; }

		[Required]
		public string QuantityPerUnit { get; set; }

		public decimal? UnitPrice { get; set; }

		public short UnitsInStock { get; set; }

		public short? UnitsOnOrder { get; set; }

		public short? ReorderLevel { get; set; }

		public bool Discontinued { get; set; }

		public ICollection<OrderDetail> OrderDetails { get; set; }
	}
}