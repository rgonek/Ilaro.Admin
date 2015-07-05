using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Filters
{
    public class ForeignEntityFilter : IEntityFilter
    {
        public Property Property { get; set; }

        public SelectList Options { get; set; }

        public string Value { get; set; }

        public ForeignEntityFilter(Property property, string value = "")
        {
            Initialize(property, value);
        }

        public void Initialize(Property property, string value = "")
        {
            Value = value ?? String.Empty;

            Property = property;
        }

        public string GetSqlCondition(string alias, ref List<object> args)
        {
            var sql = string.Format("{0}[{1}] = @{2}", alias, Property.ColumnName, args.Count);
            args.Add(Value);
            return sql;
        }
    }
}