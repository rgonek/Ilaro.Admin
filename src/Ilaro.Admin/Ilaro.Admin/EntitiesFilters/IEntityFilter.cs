using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.EntitiesFilters
{
	public interface IEntityFilter
	{
		Property Property { get; set; }

		SelectList Options { get; set; }

		string Value { get; set; }

		void Initialize(Property property, string value = "");

		string GetSQLCondition(string alias);
	}
}