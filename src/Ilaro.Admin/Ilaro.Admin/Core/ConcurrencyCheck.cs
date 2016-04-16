using System;
using System.Globalization;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public static class ConcurrencyCheck
    {
        public static object Convert(string concurrencyCheck, Entity entity)
        {
            if (entity.ConcurrencyCheckEnabled == false ||
                concurrencyCheck == null)
                return null;

            var property = entity.Properties.FirstOrDefault(x => x.IsConcurrencyCheck);
            if (property == null)
            {
                return DateTime.Parse(concurrencyCheck, CultureInfo.CurrentCulture);
            }

            if (property.IsConcurrencyCheck && property.TypeInfo.Type == typeof(byte[]))
                return System.Convert.FromBase64String(concurrencyCheck);

            return System.Convert.ChangeType(
                concurrencyCheck, 
                property.TypeInfo.Type, 
                CultureInfo.CurrentCulture);
        }
    }
}