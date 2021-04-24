﻿using Ilaro.Admin.Core.DataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

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
                concurrencyCheckProperty.Raw :
                DateTime.UtcNow;
        }

        public static object CreateInstance(this EntityRecord entityRecord)
        {
            var instance = Activator.CreateInstance(entityRecord.Entity.Type, null);

            FillPropertiesValues(instance, entityRecord);

            return instance;
        }

        private static void FillPropertiesValues(object instance, EntityRecord entityRecord)
        {
            foreach (var propertyValue in GetPropertiesValuesToFill(entityRecord))
            {
                var value = propertyValue.AsObject;
                if (IsFile(propertyValue))
                {
                    value = (value as IFormFile).FileName;
                }
                var propertyInfo = entityRecord.Entity.Type
                    .GetProperty(propertyValue.Property.Name);
                propertyInfo.SetValue(instance, value);
            }
        }

        private static IEnumerable<PropertyValue> GetPropertiesValuesToFill(
            EntityRecord entityRecord)
        {
            return entityRecord.Values
                .Where(value =>
                    value.Raw != null &&
                    (value.Raw is ValueBehavior) == false &&
                    !value.Property.IsForeignKey ||
                    (value.Property.IsForeignKey && value.Property.TypeInfo.IsSystemType));
        }

        private static bool IsFile(PropertyValue value)
        {
            return value.Property.TypeInfo.IsFile &&
                value.Property.TypeInfo.IsFileStoredInDb == false
                && value.AsObject is IFormFile;
        }

        public static void SetKeyValue(this EntityRecord entityRecord, string key)
        {
            var keys = key.Split(Id.ColumnSeparator).Select(x => x.Trim()).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                entityRecord.Id[i].ToObject(keys[i]);
            }
        }
    }
}