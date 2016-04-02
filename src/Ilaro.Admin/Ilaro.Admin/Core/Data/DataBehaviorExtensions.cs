using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core.Data
{
    public static class DataBehaviorExtensions
    {
        public static IEnumerable<PropertyValue> WhereIsNotSkipped(
            this IEnumerable<PropertyValue> propertiesValues)
        {
            return propertiesValues.Where(value => value.Raw.IsBehavior(DataBehavior.Skip) == false);
        }

        public static bool IsBehavior(this object val, DataBehavior behavior)
        {
            return val is DataBehavior && (DataBehavior)val == behavior;
        }

        public static bool IsBehavior(this object val, ValueBehavior behavior)
        {
            return val is ValueBehavior && (ValueBehavior)val == behavior;
        }
    }
}