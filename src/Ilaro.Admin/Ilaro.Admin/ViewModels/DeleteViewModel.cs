using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.ViewModels
{
	public class DeleteViewModel
	{
		public EntityViewModel Entity { get; set; }

		public string EntityName { get; set; }

		public string Key { get; set; }

		public IList<PropertyDeleteViewModel> PropertiesDeleteOptions { get; set; }

		public RecordHierarchy RecordHierarchy { get; set; }
	}
}