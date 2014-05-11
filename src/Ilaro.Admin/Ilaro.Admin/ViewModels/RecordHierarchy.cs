using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
	public class RecordHierarchy
	{
		public string KeyValue { get; set; }

		public string DisplayName { get; set; }

		public EntityViewModel Entity { get; set; }

		public IList<RecordHierarchy> SubRecordsHierarchies { get; set; }
	}
}