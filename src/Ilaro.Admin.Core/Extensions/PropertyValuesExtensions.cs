using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core.Extensions
{
    public static class PropertyValuesExtensions
    {
        public static MultiSelectList GetPossibleValues(
            this PropertyValue propertyValue,
            bool addChooseItem = true)
        {
            if (propertyValue.Property.IsForeignKey)
            {
                var options = new Dictionary<string, string>();

                if (addChooseItem)
                {
                    options.Add(string.Empty, "Choose");
                }
                options = options.Union(propertyValue.PossibleValues).ToDictionary(x => x.Key, x => x.Value);

                return propertyValue.Property.TypeInfo.IsCollection ?
                    new MultiSelectList(options, "Key", "Value", propertyValue.Values) :
                    new SelectList(options, "Key", "Value", propertyValue.AsString);
            }
            else
            {
                var options = addChooseItem ?
                    propertyValue.Property.TypeInfo.EnumType.GetOptions(string.Empty, "Choose") :
                    propertyValue.Property.TypeInfo.EnumType.GetOptions();

                if (propertyValue.Property.TypeInfo.IsEnum)
                {
                    return new SelectList(
                        options,
                        "Key",
                        "Value",
                        propertyValue.AsObject);
                }

                return new SelectList(options, "Key", "Value", propertyValue.AsString);
            }
        }

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