using System.Collections.Generic;

namespace Ilaro.Admin.Core.Filters
{
    public abstract class BaseFilter
    {
        public abstract Property Property { get; protected set; }
        public abstract IList<TemplatedSelectListItem> Options { get; protected set; }
        public abstract string Value { get; protected set; }
        public abstract bool DisplayInUI { get; }
        public bool IsActive { get { return string.IsNullOrWhiteSpace(Value) == false; } }
        public bool IsVisible
        {
            get
            {
                return DisplayInUI && Property.IsFilterable;
            }
        }

        public BaseFilter(Property property, string value = "")
        {
            Value = value ?? string.Empty;
            Property = property;
            Options = new List<TemplatedSelectListItem>();
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