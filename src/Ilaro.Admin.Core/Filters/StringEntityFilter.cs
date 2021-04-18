using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core.Extensions;
using Resources;
using SqlKata;

namespace Ilaro.Admin.Core.Filters
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

            Options.Add(new TemplatedSelectListItem(IlaroAdminResources.All, Const.EmptyFilterValue, Value, additionalMatchValues: string.Empty));

            //if (Options.Any(x => x.Selected))
            //    Options.Add(new TemplatedSelectListItem(string.Empty, Value, string.Empty, Templates.Filter.String));
            //else
            //    Options.Add(new TemplatedSelectListItem(string.Empty, Value, Value, Templates.Filter.String));
        }

        public override string GetSqlCondition(string alias, ref List<object> args)
        {
            var sql = "{0}{1} LIKE @{2}".Fill(alias, Property.Column, args.Count);
            var decoratedValue = "%{0}%".Fill(Value);
            args.Add(decoratedValue);

            return sql;
        }

        public override void AddCondition(Query query)
            => query.WhereLike(Property.Column, Value);
    }
}