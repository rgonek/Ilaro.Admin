using Ilaro.Admin.Core.Data;
using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OnCreateAttribute : Attribute
    {
        public virtual object Value { get; }

        public OnCreateAttribute(ValueBehavior valueBehavior)
        {
            Value = valueBehavior;
        }

        public OnCreateAttribute(object value)
        {
            Value = value;
        }
    }
}