using Ilaro.Admin.Core.Data;
using System;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultFilterAttribute : Attribute
    {
        public virtual object Value { get; }

        public DefaultFilterAttribute(ValueBehavior valueBehavior)
        {
            Value = valueBehavior;
        }

        public DefaultFilterAttribute(object value)
        {
            Value = value;
        }
    }
}