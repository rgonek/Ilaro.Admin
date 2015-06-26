using System.Collections.Generic;

namespace Ilaro.Admin.ViewModels
{
	public class RecordHierarchy
	{
		public string KeyValue { get; set; }

		public string DisplayName { get; set; }

		public Entity Entity { get; set; }

		public IList<RecordHierarchy> SubRecordsHierarchies { get; set; }
	}
}