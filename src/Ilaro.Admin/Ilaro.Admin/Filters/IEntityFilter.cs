using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Filters
{
    public interface IEntityFilter
    {
        Property Property { get; }

        SelectList Options { get; }

        string Value { get; }

        //void Initialize(Property property, string value = "");

        string GetSqlCondition(string alias, ref List<object> args);
    }
}