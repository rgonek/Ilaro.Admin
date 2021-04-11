using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core.Extensions
{
    public static class PropertyValuesExtensions
    {
        public static IEnumerable<PropertyValue> WhereIsNotOneToMany(
            this IEnumerable<PropertyValue> propertiesValues)
        {
            return propertiesValues
                .Where(value =>
                    (value.Property.IsForeignKey &&
                    value.Property.TypeInfo.IsCollection) == false);
        }

        public static IEnumerable<PropertyValue> WhereOneToMany(
            this IEnumerable<PropertyValue> propertiesValues)
        {
            return propertiesValues
                .Where(value =>
                    value.Property.IsForeignKey &&
                    value.Property.TypeInfo.IsCollection &&
                    value.Property.ForeignEntity != null);
        }
    }
}