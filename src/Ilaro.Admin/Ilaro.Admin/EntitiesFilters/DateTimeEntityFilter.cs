using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.ViewModels;
using Resources;

namespace Ilaro.Admin.EntitiesFilters
{
	public class DateTimeEntityFilter : IEntityFilter
	{
		public Property Property { get; set; }

		public SelectList Options { get; set; }

		public string Value { get; set; }

		public void Initialize(Property property, string value = "")
		{
			Value = value ?? String.Empty;

			Property = property;

			var options = new Dictionary<string, string>
            {
                { IlaroAdminResources.All, String.Empty },
                { IlaroAdminResources.Today, DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.Yesterday, DateTime.Today.AddDays(-1).ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastWeek, DateTime.Today.AddDays(-7).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastMonth, DateTime.Today.AddMonths(-1).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastQuarter, DateTime.Today.AddMonths(-3).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastHalfAYear, DateTime.Today.AddMonths(-6).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastYear, DateTime.Today.AddYears(-1).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") }
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
						return string.Format("({0}[{1}] >= '{2}' AND {0}[{1}] <= '{3}')", alias, Property.ColumnName, dates[0], dates[1]);
					}
					else if (dates[0].IsNullOrEmpty() && !dates[1].IsNullOrEmpty())
					{
						return string.Format("{0}[{1}] <= '{2}'", alias, Property.ColumnName, dates[1]);
					}
					else if (!dates[0].IsNullOrEmpty() && dates[1].IsNullOrEmpty())
					{
						return string.Format("{0}[{1}] >= '{2}'", alias, Property.ColumnName, dates[0]);
					}
				}

				return null;
			}
			else
			{
				return string.Format("{0}[{1}] = '{2}'", alias, Property.ColumnName, Value);
			}
		}
	}
}