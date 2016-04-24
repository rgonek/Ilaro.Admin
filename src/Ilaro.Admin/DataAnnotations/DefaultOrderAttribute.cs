using Ilaro.Admin.Core;
using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultOrderAttribute : Attribute
    {
        public virtual OrderType OrderType { get; }

        public DefaultOrderAttribute(OrderType orderType = OrderType.Asc)
        {
            OrderType = orderType;
        }
    }
}