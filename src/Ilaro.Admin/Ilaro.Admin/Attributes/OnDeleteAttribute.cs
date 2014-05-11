using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class OnDeleteAttribute : Attribute
	{
		public DeleteOption DeleteOption { get; set; }

		public OnDeleteAttribute(DeleteOption deleteOption)
		{
			DeleteOption = deleteOption;
		}
	}
}