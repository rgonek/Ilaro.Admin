using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Extensions
{
	public static class EnumExtensions
	{
		public static IDictionary<string, string> GetOptions(this Type type, string key = "", string value = "")
		{
			if (!type.IsEnum)
			{
				return null;
			}

			var dict = new Dictionary<string, string>();

			if (!key.IsNullOrEmpty() || !value.IsNullOrEmpty())
			{
				dict.Add(key, value);
			}

			foreach (Enum item in Enum.GetValues(type))
			{
				// TODO: Localize Enums
				//dict.Add(Convert.ToInt32(item).ToString(), item.GetDescription() ?? item.ToString().SplitCamelCase());
				dict.Add(Convert.ToInt32(item).ToString(), item.ToString().SplitCamelCase());
			}

			return dict;
		}
	}
}