using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.EntitiesFilters
{
	public class ForeignEntityFilter : IEntityFilter
	{
		public PropertyViewModel Property { get; set; }

		public SelectList Options { get; set; }

		public string Value { get; set; }

		public ForeignEntityFilter(PropertyViewModel property, string value = "")
		{
			Initialize(property, value);
		}

		public void Initialize(PropertyViewModel property, string value = "")
		{
			Value = value ?? String.Empty;

			Property = property;
		}

		public string GetSQLCondition(string alias)
		{
			return string.Format("{0}[{1}] = {2}", alias, Property.ColumnName, Value);
		}
	}
}