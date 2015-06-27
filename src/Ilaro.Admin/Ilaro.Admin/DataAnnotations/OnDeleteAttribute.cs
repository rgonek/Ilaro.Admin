using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.DataAnnotations
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