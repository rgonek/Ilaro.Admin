using System.Collections.Generic;
using Ilaro.Admin.Core.Extensions;
using Resources;

namespace Ilaro.Admin.Core.Filters
{
    public class BoolEntityFilter : BaseFilter<bool>
    {
        public override Property Property { get; protected set; }

        public override sealed IList<TemplatedSelectListItem> Options { get; protected set; }

        public override sealed string Value { get; protected set; }

        public override bool DisplayInUI { get { return true; } }

        public BoolEntityFilter(Property property, string value = "")
            : base(property, value)
        {

            Options.Add(new TemplatedSelectListItem(IlaroAdminResources.All, Const.EmptyFilterValue, Value, additionalMatchValues: string.Empty));
            Options.Add(new TemplatedSelectListItem(IlaroAdminResources.Yes, "1", Value, null, true.ToString(), "t"));
            Options.Add(new TemplatedSelectListItem(IlaroAdminResources.No, "0", Value, null, false.ToString(), "f"));
        }

        public override string GetSqlCondition(string alias, ref List<object> args)
        {
            var sql = "{0}{1} = @{2}".Fill(alias, Property.Column, args.Count);
            args.Add(Value);

            return sql;
        }
    }
}