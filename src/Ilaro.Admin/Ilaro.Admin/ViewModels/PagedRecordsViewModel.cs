using System.Collections.Generic;

namespace Ilaro.Admin.ViewModels
{
	public class PagedRecordsViewModel
	{
		public int TotalItems { get; set; }

		public int TotalPages { get; set; }

		public IList<DataRowViewModel> Records { get; set; }
	}
}