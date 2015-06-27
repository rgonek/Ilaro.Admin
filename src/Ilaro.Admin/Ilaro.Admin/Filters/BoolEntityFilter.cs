using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Resources;

namespace Ilaro.Admin.Filters
{
    public class BoolEntityFilter : IEntityFilter
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
                { IlaroAdminResources.Yes, "1" },
                { IlaroAdminResources.No, "0" }
            };

            Options = new SelectList(options, "Value", "Key", Value);
        }

        public string GetSqlCondition(string alias)
        {
            return string.Format("{0}[{1}] = {2}", alias, Property.ColumnName, Value);
        }
    }
}