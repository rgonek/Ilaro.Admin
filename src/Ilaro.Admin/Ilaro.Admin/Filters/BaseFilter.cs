using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Filters
{
    public abstract class BaseFilter : IEntityFilter
    {
        public abstract Property Property { get; protected set; }
        public abstract SelectList Options { get; protected set; }
        public abstract string Value { get; protected set; }
        public abstract bool DisplayInUI { get; }
        public bool IsActive { get { return string.IsNullOrWhiteSpace(Value) == false; } }

        public BaseFilter(Property property, string value = "")
        {
            Value = value ?? String.Empty;
            Property = property;
        }

        public abstract string GetSqlCondition(string alias, ref List<object> args);
    }

    public abstract class BaseFilter<T> : BaseFilter
    {
        protected BaseFilter(Property property, string value = "")
            : base(property, value)
        {
        }
    }
}