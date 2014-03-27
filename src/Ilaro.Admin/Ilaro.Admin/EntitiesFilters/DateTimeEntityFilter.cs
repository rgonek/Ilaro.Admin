using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.ViewModels;

namespace Ilaro.Admin.EntitiesFilters
{
	public class DateTimeEntityFilter : IEntityFilter
	{
		public PropertyViewModel Property { get; set; }

		public SelectList Options { get; set; }

		public string Value { get; set; }

		public void Initialize(PropertyViewModel property, string value = "")
		{
			Value = value ?? String.Empty;

			Property = property;

			// TODO: localize strings
			var options = new Dictionary<string, string>
            {
                { "All", String.Empty },
                { "Last day", "2013.01.17" },
                { "Last weekd", "2013.01.11-2013.01.18" },
                { "Last month", "2012.12.18-2013.01.18" },
                { "Last quarter", "2012.09.18-2013.01.18" },
                { "Last half a year", "2012.06.18-2013.01.18" },
                { "Last year", "2012.01.18-2013.01.18" }
            };

			Options = new SelectList(options, "Value", "Key", Value);
		}

		public string GetSQLCondition(string alias)
		{
			if (Value.Contains('-'))
			{
				var dates = Value.Split("-".ToCharArray());

				if (dates.Length == 2)
				{
					if (!dates[0].IsNullOrEmpty() && !dates[1].IsNullOrEmpty())
					{
						return string.Format("({0}[{1}] >= '{2}' AND {0}[{1}] <= '{3}')", alias, Property.Name, dates[0], dates[1]);
					}
					else if (dates[0].IsNullOrEmpty() && !dates[1].IsNullOrEmpty())
					{
						return string.Format("{0}[{1}] <= '{2}'", alias, Property.Name, dates[1]);
					}
					else if (!dates[0].IsNullOrEmpty() && dates[1].IsNullOrEmpty())
					{
						return string.Format("{0}[{1}] >= '{2}'", alias, Property.Name, dates[0]);
					}
				}

				return null;
			}
			else
			{
				return string.Format("{0}[{1}] = '{2}'", alias, Property.Name, Value);
			}
		}
	}
}