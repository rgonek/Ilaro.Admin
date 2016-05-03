using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.Core
{
    public class EntityRecord
    {
        public Entity Entity { get; }

        public IList<PropertyValue> Values { get; } = new List<PropertyValue>();

        public IEnumerable<PropertyValue> DisplayValues
        {
            get
            {
                return Values.Where(x => x.Property.IsVisible);
            }
        }

        public IList<PropertyValue> Keys
        {
            get
            {
                return Values.Where(value => value.Property.IsKey).ToList();
            }
        }

        public string JoinedKeysWithNames
        {
            get
            {
                return string.Join(
                    Const.KeyColSeparator.ToString(),
                    Keys.Select(value => string.Format("{0}={1}", value.Property.Name, value.AsString)));
            }
        }

        public string JoinedKeysValues
        {
            get { return string.Join(Const.KeyColSeparator.ToString(), Keys.Select(x => x.AsString)); }
        }

        public PropertyValue ConcurrencyCheck
        {
            get
            {
                return Values.FirstOrDefault(x => x.Property.IsConcurrencyCheck);
            }
        }

        public PropertyValue this[string propertyName]
        {
            get { return Values.FirstOrDefault(x => x.Property.Name == propertyName); }
        }

        public EntityRecord(Entity entity)
        {
            Entity = entity;
        }

        internal static EntityRecord CreateEmpty(Entity entity)
        {
            var entityRecord = new EntityRecord(entity);
            entityRecord.Fill(new Dictionary<string, object>());
            return entityRecord;
        }

        public void Fill(
            IValueProvider valueProvider,
            HttpFileCollectionBase files,
            Func<Property, object> defaultValueResolver = null)
        {
            foreach (var property in Entity.Properties.DistinctBy(x => x.Column))
            {
                var propertyValue = new PropertyValue(property);
                Values.Add(propertyValue);
                if (property.TypeInfo.IsFile)
                {
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
                }
                else
                {
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
                            var dateString = (string)value.ConvertTo(typeof(string));
                            DateTime dateTime;
                            DateTime.TryParseExact(
                                dateString,
                                property.GetDateTimeFormat(),
                                CultureInfo.CurrentCulture,
                                DateTimeStyles.None,
                                out dateTime);
                            if (dateTime == DateTime.MinValue)
                            {
                                DateTime.TryParseExact(
                                    dateString,
                                    property.GetDateFormat(),
                                    CultureInfo.CurrentCulture,
                                    DateTimeStyles.None,
                                    out dateTime);
                            }

                            propertyValue.Raw = dateTime;
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
                }
            }
        }

        public void Fill(
            IDictionary<string, object> item,
            string prefix = "",
            Func<object, object> valueMutator = null)
        {
            foreach (var property in Entity.Properties)
            {
                var value = item.ContainsKey(prefix + property.Column.Undecorate()) ?
                        item[prefix + property.Column.Undecorate()] :
                        null;
                if (valueMutator != null)
                    value = valueMutator(value);
                Values.Add(new PropertyValue(property)
                {
                    Raw = value
                });
            }
        }

        internal void Fill(
            NameValueCollection request,
            string prefix = "",
            Func<object, object> valueMutator = null)
        {
            Fill(
                request.ToDictionary().ToDictionary(x => x.Key, x => (object)x.Value),
                prefix,
                valueMutator);
        }

        public void Fill(
            string key,
            IValueProvider collection,
            HttpFileCollectionBase files,
            Func<Property, object> defaultValueResolver = null)
        {
            Fill(collection, files, defaultValueResolver);
            SetKeyValue(key);
        }

        public void SetKeyValue(string key)
        {
            var keys = key.Split(Const.KeyColSeparator).Select(x => x.Trim()).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                Keys[i].ToObject(keys[i]);
            }
        }

        /// <summary>
        /// Get display name for record
        /// </summary>
        public override string ToString()
        {
            // check if has to string attribute
            if (Entity.RecordDisplayFormat.HasValue())
            {
                var result = Entity.RecordDisplayFormat;
                foreach (var PropertyValue in Values)
                {
                    result = result.Replace("{" + PropertyValue.Property.Name + "}", PropertyValue.AsString);
                }

                return result;
            }
            // if not check if has ToString() method
            if (Entity.HasToStringMethod)
            {
                var methodInfo = Entity.Type.GetMethod("ToString");
                var instance = this.CreateInstance();

                var result = methodInfo.Invoke(instance, null);

                return result.ToStringSafe();
            }

            return null;
        }
    }
}