using Ilaro.Admin.Core;
using System.Globalization;

namespace Ilaro.Admin.Extensions
{
    public static class PropertyExtensions
    {
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