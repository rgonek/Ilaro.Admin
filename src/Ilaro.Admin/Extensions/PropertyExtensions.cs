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
    }
}