using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Model
{
	public enum EntityChangeType : byte
	{
		Insert = 0,
		Update = 1,
		Delete = 2
	}
}