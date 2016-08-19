using Ilaro.Admin.Core.Data;
using System;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OnSaveAttribute : Attribute
    {
        public virtual object Value { get; }

        public OnSaveAttribute(ValueBehavior valueBehavior)
        {
            Value = valueBehavior;
        }

        public OnSaveAttribute(object value)
        {
            Value = value;
        }
    }
}