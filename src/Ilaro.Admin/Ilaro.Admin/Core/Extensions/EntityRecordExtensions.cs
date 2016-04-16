using System;

namespace Ilaro.Admin.Core.Extensions
{
    public static class EntityRecordExtensions
    {
        public static object GetConcurrencyCheckValue(
            this EntityRecord entityRecord, 
            object concurrencyCheckValue = null)
        {
            if (entityRecord.Entity.ConcurrencyCheckEnabled == false)
                return null;

            var concurrencyCheckProperty = entityRecord.ConcurrencyCheck;

            return concurrencyCheckProperty != null ?
                concurrencyCheckProperty.AsObject :
                concurrencyCheckValue ?? DateTime.UtcNow;
        }
    }
}