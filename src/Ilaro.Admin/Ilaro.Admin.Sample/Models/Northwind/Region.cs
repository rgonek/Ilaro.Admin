using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Sample.Models.Northwind
{
	[Table("Region")]
	public class Region
	{
		public int RegionID { get; set; }

		[Required, MaxLength(50)]
		public string RegionDescription { get; set; }
	}
}