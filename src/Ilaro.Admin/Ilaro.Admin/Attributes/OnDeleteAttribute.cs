using System;
using Ilaro.Admin.ViewModels;

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