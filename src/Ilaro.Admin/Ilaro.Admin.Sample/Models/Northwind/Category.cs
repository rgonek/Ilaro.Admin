using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Sample.Models.Northwind
{
	public class Category
	{
		public int CategoryID { get; set; }

		[StringLength(15)]
		public string CategoryName { get; set; }

		public string Description { get; set; }

		//public byte[] Picture { get; set; }

		public ICollection<Product> Products { get; set; }
	}
}