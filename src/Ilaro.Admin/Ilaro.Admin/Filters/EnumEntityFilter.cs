using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using Resources;

namespace Ilaro.Admin.Filters
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
                { IlaroAdminResources.All, String.Empty }
            };

            foreach (var option in property.TypeInfo.EnumType.GetOptions())
            {
                options.Add(option.Value, option.Key);
            }

            Options = new SelectList(options, "Value", "Key", Value);
        }

        public string GetSqlCondition(string alias, ref List<object> args)
        {
            var sql = "{0}[{1}] = @{2}".Fill(alias, Property.ColumnName, args.Count);
            args.Add(Value);
            return sql;
        }
    }
}