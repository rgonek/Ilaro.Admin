using System.Web.Mvc;
using Ilaro.Admin.ViewModels;

namespace Ilaro.Admin.EntitiesFilters
{
	public interface IEntityFilter
	{
		Property Property { get; set; }

		SelectList Options { get; set; }

		string Value { get; set; }

		void Initialize(Property property, string value = "");

		string GetSqlCondition(string alias);
	}
}