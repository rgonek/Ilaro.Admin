using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core.Data
{
    public static class DataBehaviorExtensions
    {
        public static IEnumerable<Property> WhereIsNotSkipped(this IEnumerable<Property> properties)
        {
            return properties.Where(x => x.Value.Raw.IsBehavior(DataBehavior.Skip) == false);
        }

        public static bool IsBehavior(this object val, DataBehavior behavior)
        {
            return val is DataBehavior && (DataBehavior)val == behavior;
        }
    }
}