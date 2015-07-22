using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using Resources;

namespace Ilaro.Admin.Filters
{
    public class BoolEntityFilter : BaseFilter<bool>
    {
        public override Property Property { get; protected set; }
        public override sealed SelectList Options { get; protected set; }
        public override sealed string Value { get; protected set; }
        public override bool DisplayInUI { get { return true; } }

        public BoolEntityFilter(Property property, string value = "")
            : base(property, value)
        {
            var options = new Dictionary<string, string>
            {
                { IlaroAdminResources.All, String.Empty },
                { IlaroAdminResources.Yes, "1" },
                { IlaroAdminResources.No, "0" }
            };

            Options = new SelectList(options, "Value", "Key", Value);
        }

        public override string GetSqlCondition(string alias, ref List<object> args)
        {
            var sql = "{0}{1} = @{2}".Fill(alias, Property.ColumnName, args.Count);
            args.Add(Value);

            return sql;
        }
    }
}