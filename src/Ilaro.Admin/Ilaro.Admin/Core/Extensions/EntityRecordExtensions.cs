using System;
using System.Globalization;

namespace Ilaro.Admin.Core.Extensions
{
    public static class EntityRecordExtensions
    {
        public static string GetConcurrencyCheckValue(this EntityRecord entityRecord)
        {
            if (entityRecord.Entity.ConcurrencyCheckEnabled == false)
                return null;

            var concurrencyCheckProperty = entityRecord.ConcurrencyCheck;

            return concurrencyCheckProperty != null ?
                concurrencyCheckProperty.AsString :
                DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }
    }
}