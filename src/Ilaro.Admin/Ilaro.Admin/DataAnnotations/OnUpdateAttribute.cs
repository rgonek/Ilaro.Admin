using Ilaro.Admin.Core.Data;
using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OnUpdateAttribute : Attribute
    {
        public virtual object Value { get; }

        public OnUpdateAttribute(ValueBehavior valueBehavior)
        {
            Value = valueBehavior;
        }

        public OnUpdateAttribute(object value)
        {
            Value = value;
        }
    }
}