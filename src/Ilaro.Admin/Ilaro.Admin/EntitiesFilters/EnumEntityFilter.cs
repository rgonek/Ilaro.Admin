using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.EntitiesFilters
{
	public class EnumEntityFilter : IEntityFilter
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
                { "Wszystkie", String.Empty }
            };

			foreach (var option in property.EnumType.GetOptions())
			{
				options.Add(option.Value, option.Key.ToString());
			}

			Options = new SelectList(options, "Value", "Key", Value);
		}

		public string GetSQLCondition(string alias)
		{
			return string.Format("{0}[{1}] = {2}", alias, Property.ColumnName, Value);
		}
	}
}