using System;

namespace Ilaro.Admin.Core.Extensions
{
    public static class EntityRecordExtensions
    {
        public static object GetConcurrencyCheckValue(this EntityRecord entityRecord)
        {
            if (entityRecord.Entity.ConcurrencyCheckEnabled == false)
                return null;

            var concurrencyCheckProperty = entityRecord.ConcurrencyCheck;

            return concurrencyCheckProperty != null ?
                concurrencyCheckProperty.AsObject :
                DateTime.UtcNow;
        }
    }
}