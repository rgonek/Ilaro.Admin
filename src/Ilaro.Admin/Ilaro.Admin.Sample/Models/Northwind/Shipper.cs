using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Sample.Models.Northwind
{
	public class Shipper
	{
		public int ShipperID { get; set; }

		[Required, MaxLength(40)]
		public string CompanyName { get; set; }

		[MaxLength(24)]
		public string Phone { get; set; }
	}
}