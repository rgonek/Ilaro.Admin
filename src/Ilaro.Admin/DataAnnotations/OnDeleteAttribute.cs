using Ilaro.Admin.Core.Data;
using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OnDeleteAttribute : Attribute
    {
        public virtual object Value { get; }

        /// <summary>
        /// Used only when soft delete for entity is enabled
        /// </summary>
        public OnDeleteAttribute(ValueBehavior valueBehavior)
        {
            Value = valueBehavior;
        }

        /// <summary>
        /// Used only when soft delete for entity is enabled
        /// </summary>
        public OnDeleteAttribute(object value)
        {
            Value = value;
        }
    }
}