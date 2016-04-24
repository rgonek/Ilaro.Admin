using Ilaro.Admin.Core;
using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultOrder : Attribute
    {
        public virtual OrderType OrderType { get; }

        public DefaultOrder(OrderType orderType = OrderType.Asc)
        {
            OrderType = orderType;
        }
    }
}