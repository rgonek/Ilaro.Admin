using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core.Data
{
    public static class DataBehaviorExtensions
    {
        public static IEnumerable<PropertyValue> WhereIsNotSkipped(
            this IEnumerable<PropertyValue> propertiesValues)
        {
            return propertiesValues.Where(value => value.DataBehavior != DataBehavior.Skip);
        }

        public static bool IsBehavior(this object val, ValueBehavior behavior)
        {
            return val is ValueBehavior && (ValueBehavior)val == behavior;
        }
    }
}