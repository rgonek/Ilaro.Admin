﻿using System.Collections.Generic;
using Ilaro.Admin.Core.Extensions;

namespace Ilaro.Admin.Core.Filters
{
    public class ForeignEntityFilter : BaseFilter
    {
        public override Property Property { get; protected set; }

        public override sealed IList<TemplatedSelectListItem> Options { get; protected set; }

        public override sealed string Value { get; protected set; }

        public override bool DisplayInUI { get { return false; } }

        public ForeignEntityFilter(Property property, string value = "")
            : base(property, value)
        {
        }

        public override string GetSqlCondition(string alias, ref List<object> args)
        {
            var sql = "{0}{1} = @{2}".Fill(alias, Property.Column, args.Count);
            args.Add(Value);
            return sql;
        }
    }
}