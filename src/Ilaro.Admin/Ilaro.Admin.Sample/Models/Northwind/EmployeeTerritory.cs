using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Sample.Models.Northwind
{
	[Table("EmployeeTerritories")]
	public class EmployeeTerritory
	{
		[Key]
		public int EmployeeID { get; set; }

		[Required, ForeignKey("TerritoryID")]
		public Territory Territory { get; set; }
	}
}