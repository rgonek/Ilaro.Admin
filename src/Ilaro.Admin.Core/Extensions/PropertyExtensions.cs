using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ilaro.Admin.Core.Extensions
{
    public static class PropertyExtensions
    {
        //public static MultiSelectList GetPossibleValues(
        //    this PropertyValue propertyValue,
        //    bool addChooseItem = true)
        //{
        //    if (propertyValue.Property.IsForeignKey)
        //    {
        //        var options = new Dictionary<string, string>();

        //        if (addChooseItem)
        //        {
        //            options.Add(String.Empty, IlaroAdminResources.Choose);
        //        }
        //        options = options.Union(propertyValue.PossibleValues).ToDictionary(x => x.Key, x => x.Value);

        //        return propertyValue.Property.TypeInfo.IsCollection ?
        //            new MultiSelectList(options, "Key", "Value", propertyValue.Values) :
        //            new SelectList(options, "Key", "Value", propertyValue.AsString);
        //    }
        //    else
        //    {
        //        var options = addChooseItem ?
        //            propertyValue.Property.TypeInfo.EnumType.GetOptions(String.Empty, IlaroAdminResources.Choose) :
        //            propertyValue.Property.TypeInfo.EnumType.GetOptions();

        //        if (propertyValue.Property.TypeInfo.IsEnum)
        //        {
        //            return new SelectList(
        //                options,
        //                "Key",
        //                "Value",
        //                propertyValue.AsObject);
        //        }

        //        return new SelectList(options, "Key", "Value", propertyValue.AsString);
        //    }
        //}

        public static IEnumerable<Property> WhereOneToMany(
            this IEnumerable<Property> propertiesValues)
        {
            return propertiesValues
                .Where(property =>
                    property.IsForeignKey &&
                    property.TypeInfo.IsCollection &&
                    property.ForeignEntity != null);
        }

        public static IEnumerable<Property> SkipOneToMany(
            this IEnumerable<Property> propertiesValues)
        {
            return propertiesValues
                .Where(property =>
                    (property.IsForeignKey &&
                    property.TypeInfo.IsCollection) == false);
        }


        internal static bool GetDefaultVisibility(this Property property)
        {
            // Get all properties which is not a key and foreign key
            if (!property.IsForeignKey)
            {
                return true;
            }
            else if (property.IsForeignKey)
            {
                // If is foreign key and not have reference property
                if (
                    property.ReferenceProperty == null &&
                    !property.TypeInfo.IsCollection)
                {
                    return true;
                }
                // If is foreign key and have foreign key, that means,
                // we have two properties for one database column,
                // so I want only that one who is a system type
                else if (
                    property.ReferenceProperty != null &&
                    property.TypeInfo.IsSystemType &&
                    !property.TypeInfo.IsCollection)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFormat(this Property property)
        {
            if (property.Format.HasValue())
                return property.Format;

            if (property.TypeInfo.DataType == DataType.DateTime)
                return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " +
                    CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

            return string.Empty;
        }

        public static string GetDateFormat(this Property property)
        {
            if (property.Format.HasValue())
                return property.Format;

            if (property.TypeInfo.DataType == DataType.DateTime)
                return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

            return string.Empty;
        }

        public static string GetTimeFormat(this Property property)
        {
            if (property.Format.HasValue())
                return property.Format;

            if (property.TypeInfo.DataType == DataType.DateTime)
                return CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

            return string.Empty;
        }

        public static string GetDateTimeFormat(this Property property)
        {
            if (property.Format.HasValue())
                return property.Format;

            if (property.TypeInfo.DataType == DataType.DateTime)
                return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " +
                    CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

            return string.Empty;
        }

        public static string GetUIDateFormat(this Property property)
        {
            return DotNetToMomentDateTimeFormat.Convert(property.GetDateFormat());
        }

        public static string GetUITimeFormat(this Property property)
        {
            return DotNetToMomentDateTimeFormat.Convert(property.GetTimeFormat());
        }

        public static string GetUIDateTimeFormat(this Property property)
        {
            return DotNetToMomentDateTimeFormat.Convert(property.GetDateTimeFormat());
        }
    }
}