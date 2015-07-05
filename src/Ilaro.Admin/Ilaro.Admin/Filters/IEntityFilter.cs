using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Filters
{
    public interface IEntityFilter
    {
        Property Property { get; set; }

        SelectList Options { get; set; }

        string Value { get; set; }

        void Initialize(Property property, string value = "");

        string GetSqlCondition(string alias, ref List<object> args);
    }
}