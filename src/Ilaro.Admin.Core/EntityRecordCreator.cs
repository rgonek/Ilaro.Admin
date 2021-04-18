using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public class EntityRecordCreator
    {
        public static EntityRecord CreateRecord(
            Entity entity,
            IFormCollection collection,
            Func<Property, object> defaultValueResolver = null)
        {
            var entityRecord = new EntityRecord(entity);
            foreach (var property in entity.Properties.DistinctBy(x => x.Column))
            {
                var propertyValue = GetPropertyValue(property, collection, defaultValueResolver);
                entityRecord.Values.Add(propertyValue);
            }

            return entityRecord;
        }

        private static PropertyValue GetPropertyValue(
            Property property,
            IFormCollection collection,
            Func<Property, object> defaultValueResolver = null)
        {
            var propertyValue = new PropertyValue(property);

            if (collection.TryGetValue(property.Name, out StringValues value))
            {
                if (property.IsForeignKey && property.TypeInfo.IsCollection)
                {
                    propertyValue.Values = value
                        .ToArray()
                        .OfType<object>()
                        .ToList();
                }
                else if (property.TypeInfo.DataType == DataType.DateTime)
                {
                    propertyValue.Raw = ParseDateTime(property, value);
                }
                else
                {
                    propertyValue.Raw = Convert.ChangeType(
                        (string)value,
                        property.TypeInfo.OriginalType,
                        CultureInfo.CurrentCulture);
                }
            }

            if (defaultValueResolver != null)
            {
                var defaultValue = defaultValueResolver(property);

                if (defaultValue is ValueBehavior ||
                    (propertyValue.Raw == null && defaultValue != null))
                {
                    propertyValue.Raw = defaultValue;
                }
            }

            return propertyValue;
        }

        private static DateTime ParseDateTime(Property property, string date)
        {
            DateTime dateTime;
            DateTime.TryParseExact(
                date,
                property.GetDateTimeFormat(),
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out dateTime);
            if (dateTime == DateTime.MinValue)
            {
                DateTime.TryParseExact(
                    date,
                    property.GetDateFormat(),
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.None,
                    out dateTime);
            }

            return dateTime;
        }

        public static EntityRecord CreateRecord(
            Entity entity,
            IDictionary<string, object> item,
            string prefix = "",
            Func<object, object> valueMutator = null)
        {
            var entityRecord = new EntityRecord(entity);
            foreach (var property in entity.Properties)
            {
                var value = item.ContainsKey(prefix + property.Column.Undecorate()) ?
                        item[prefix + property.Column.Undecorate()] :
                        null;
                if (valueMutator != null)
                    value = valueMutator(value);
                entityRecord.Values.Add(new PropertyValue(property)
                {
                    Raw = value
                });
            }

            return entityRecord;
        }
    }
}