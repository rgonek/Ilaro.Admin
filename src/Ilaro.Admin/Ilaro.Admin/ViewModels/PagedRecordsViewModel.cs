using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
	public class PagedRecordsViewModel
	{
		public int TotalItems { get; set; }

		public int TotalPages { get; set; }

		public IList<DataRowViewModel> Records { get; set; }
	}
}