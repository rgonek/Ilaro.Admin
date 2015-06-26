using System.Collections.Generic;

namespace Ilaro.Admin.ViewModels
{
	public class DeleteViewModel
	{
		public Entity Entity { get; set; }

		public string EntityName { get; set; }

		public string Key { get; set; }

		public IList<PropertyDeleteViewModel> PropertiesDeleteOptions { get; set; }

		public RecordHierarchy RecordHierarchy { get; set; }
	}
}