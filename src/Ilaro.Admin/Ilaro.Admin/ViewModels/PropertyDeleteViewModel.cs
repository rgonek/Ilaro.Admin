using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
	public class PropertyDeleteViewModel
	{
		public string PropertyName { get; set; }

		public DeleteOption DeleteOption { get; set; }
	}
}