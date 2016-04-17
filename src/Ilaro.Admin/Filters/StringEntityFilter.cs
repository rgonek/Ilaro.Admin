using System;
using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using Resources;

namespace Ilaro.Admin.Filters
{
    public class StringEntityFilter : BaseFilter<string>
    {
        public override Property Property { get; protected set; }
        public override sealed IList<TemplatedSelectListItem> Options { get; protected set; }
        public override sealed string Value { get; protected set; }
        public override bool DisplayInUI { get { return true; } }

        public StringEntityFilter(Property property, string value = "")
            : base(property, value)
        {

            Options.Add(new TemplatedSelectListItem(IlaroAdminResources.All, String.Empty, Value));

            if (Options.Any(x => x.Selected))
                Options.Add(new TemplatedSelectListItem(String.Empty, Value, String.Empty, Templates.Filter.String));
            else
                Options.Add(new TemplatedSelectListItem(String.Empty, Value, Value, Templates.Filter.String));
        }

        public override string GetSqlCondition(string alias, ref List<object> args)
        {
            var sql = "{0}{1} LIKE @{2}".Fill(alias, Property.Column, args.Count);
            var decoratedValue = "%{0}%".Fill(Value);
            args.Add(decoratedValue);

            return sql;
        }
    }
}