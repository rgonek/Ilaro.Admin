using Ilaro.Admin.Core.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public static class EntityExtensions
    {
        internal static IEnumerable<Property> GetDefaultDisplayProperties(this Entity entity)
        {
            return entity.Properties.Where(x => x.GetDefaultVisibility());
        }

        internal static IEnumerable<Property> GetDefaultSearchProperties(this Entity entity)
        {
            return entity.Properties
                    .Where(x =>
                        !x.IsForeignKey &&
                        x.TypeInfo.IsAvailableForSearch);
        }

        internal static IEnumerable<Property> GetDefaultCreateProperties(
            this Entity entity,
            bool getKey = true,
            bool getForeignCollection = true)
        {
            var properties = entity.Properties
                .Where(x => x.IsCreatable)
                .Where(x => x.IsConcurrencyCheck == false);
            if (properties.Any(x => x.Group.HasValue()))
            {
                properties = properties.Where(x => x.Group.HasValue());
            }
            foreach (var property in properties)
            {
                // Get all properties which is not a key and foreign key
                if (!property.IsKey && !property.IsForeignKey)
                {
                    yield return property;
                }
                // If property is key
                else if (
                    property.IsKey &&
                    property.IsAutoKey == false &&
                    getKey)
                {
                    yield return property;
                }
                else if (property.IsForeignKey && property.ForeignEntity != null && property.IsKey == false)
                {
                    // If is foreign key and not have reference property
                    if (
                        property.ReferenceProperty == null &&
                        (getForeignCollection || !property.TypeInfo.IsCollection))
                    {
                        yield return property;
                    }
                    // If is foreign key and have foreign key, that means,
                    // we have two properties for one database column,
                    // so I want only that one who is a system type
                    else if (
                        property.ReferenceProperty != null &&
                        property.TypeInfo.IsSystemType &&
                        (getForeignCollection || !property.TypeInfo.IsCollection))
                    {
                        yield return property;
                    }
                }
            }
        }

        public static EntityRecord CreateEmptyRecord(this Entity entity)
        {
            return EntityRecordCreator.CreateRecord(entity, new Dictionary<string, object>());
        }

        public static EntityRecord CreateRecord(
            this Entity entity,
            IFormCollection collection,
            Func<Property, object> defaultValueResolver = null)
        {
            return EntityRecordCreator.CreateRecord(entity, collection, defaultValueResolver);
        }

        public static EntityRecord CreateRecord(
            this Entity entity,
            IDictionary<string, object> item,
            string prefix = "",
            Func<object, object> valueMutator = null)
        {
            return EntityRecordCreator.CreateRecord(entity, item, prefix, valueMutator);
        }

        public static EntityRecord CreateRecord(
            this Entity entity,
            NameValueCollection request,
            string prefix = "",
            Func<object, object> valueMutator = null)
        {
            return entity.CreateRecord(
                request.ToDictionary().ToDictionary(x => x.Key, x => (object)x.Value),
                prefix,
                valueMutator);
        }

        public static EntityRecord CreateRecord(
            this Entity entity,
            string key,
            IFormCollection collection,
            Func<Property, object> defaultValueResolver = null)
        {
            var entityRecord = entity.CreateRecord(collection, defaultValueResolver);
            entityRecord.SetKeyValue(key);

            return entityRecord;
        }
    }
}