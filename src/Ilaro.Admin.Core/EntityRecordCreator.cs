using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.DataAnnotations;
using Ilaro.Admin.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.Core
{
    public class EntityRecordCreator
    {
        public static EntityRecord CreateRecord(
            Entity entity,
            IValueProvider valueProvider,
            HttpFileCollectionBase files,
            Func<Property, object> defaultValueResolver = null)
        {
            var entityRecord = new EntityRecord(entity);
            foreach (var property in entity.Properties.DistinctBy(x => x.Column))
            {
                var propertyValue = property.TypeInfo.IsFile ?
                    GetPropertyValueForFile(property, valueProvider, files) :
                    GetPropertyValue(property, valueProvider, defaultValueResolver);
                entityRecord.Values.Add(propertyValue);
            }

            return entityRecord;
        }

        private static PropertyValue GetPropertyValue(
            Property property,
            IValueProvider valueProvider,
            Func<Property, object> defaultValueResolver = null)
        {
            var propertyValue = new PropertyValue(property);

            var value = valueProvider.GetValue(property.Name);
            if (value != null)
            {
                if (property.IsForeignKey && property.TypeInfo.IsCollection)
                {
                    propertyValue.Values = value.AttemptedValue
                        .Split(',').OfType<object>().ToList();
                }
                else if (property.TypeInfo.DataType == DataType.DateTime)
                {
                    propertyValue.Raw = ParseDateTime(property, (string)value.ConvertTo(typeof(string)));
                }
                else
                {
                    propertyValue.Raw = value.ConvertTo(
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

        private static PropertyValue GetPropertyValueForFile(
            Property property,
            IValueProvider valueProvider,
            HttpFileCollectionBase files)
        {
            var propertyValue = new PropertyValue(property);

            var file = files[property.Name];
            propertyValue.Raw = file;
            if (property.TypeInfo.IsFileStoredInDb == false &&
                property.FileOptions.NameCreation == NameCreation.UserInput)
            {
                var providedName = (string)valueProvider.GetValue(property.Name)
                    .ConvertTo(typeof(string), CultureInfo.CurrentCulture);
                propertyValue.Additional = providedName;
            }
            var isDeleted = false;

            if (file == null || file.ContentLength > 0)
            {
                isDeleted = false;
            }
            else
            {
                var isDeletedKey = property.Name + "_delete";
                if (valueProvider.ContainsPrefix(isDeletedKey))
                {
                    isDeleted =
                       ((bool?)
                           valueProvider.GetValue(isDeletedKey)
                               .ConvertTo(typeof(bool), CultureInfo.CurrentCulture)).GetValueOrDefault();
                }
            }

            if (isDeleted)
            {
                propertyValue.DataBehavior = DataBehavior.Clear;
                propertyValue.Additional = null;
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