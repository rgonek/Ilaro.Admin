using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Filters
{
    public class ChangeEntityFilter : IEntityFilter
    {
        public Property Property { get; set; }

        public SelectList Options { get; set; }

        public string Value { get; set; }

        public ChangeEntityFilter(Property property, string value = "")
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
            var sql = "{0}{1} = @{2}".Fill(alias, Property.ColumnName, args.Count);
            args.Add(Value);
            return sql;
        }
    }
}