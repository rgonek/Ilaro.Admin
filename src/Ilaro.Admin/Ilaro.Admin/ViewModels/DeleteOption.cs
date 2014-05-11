using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.ViewModels
{
	public enum DeleteOption : int
	{
		Nothing = 0,
		SetNull = 1,
		CascadeDelete = 2,
		AskUser = 3
	}
}