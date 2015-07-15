using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core.Data
{
    public static class DataBehaviorExtensions
    {
        public static IEnumerable<Property> WhereIsNotSkipped(this IEnumerable<Property> properties)
        {
            return properties.Where(x =>
                x.Value.Raw is DataBehavior == false ||
                (DataBehavior)x.Value.Raw != DataBehavior.Skip);
        }
    }
}