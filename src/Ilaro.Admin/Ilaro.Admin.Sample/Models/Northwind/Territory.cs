using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Sample.Models.Northwind
{
	public class Territory
	{
		[MaxLength(20)]
		public string TerritoryID { get; set; }

		[Required, MaxLength(50)]
		public string TerritoryDescription { get; set; }

		[Required, ForeignKey("RegionID")]
		public Region Region { get; set; }
	}
}