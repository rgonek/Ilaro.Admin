using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Models;
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

        public IList<PropertyValue> Key
        {
            get
            {
                return Values.Where(value => value.Property.IsKey).ToList();
            }
        }

        public string JoinedKeyWithValue
        {
            get
            {
                return string.Join(
                    Const.KeyColSeparator.ToString(),
                    Key.Select(value => string.Format("{0}={1}", value.Property.Name, value.AsString)));
            }
        }

        public string JoinedKeyValue
        {
            get { return string.Join(Const.KeyColSeparator.ToString(), Key.Select(x => x.AsString)); }
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
            FormCollection collection,
            HttpFileCollectionBase files,
            Func<Property, object> defaultValueResolver = null)
        {
            foreach (var property in Entity.Properties)
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
                        var providedName = (string)collection.GetValue(property.Name)
                            .ConvertTo(typeof(string), CultureInfo.CurrentCulture);
                        propertyValue.Additional = providedName;
                    }
                    var isDeleted =
                        ((bool?)
                            collection.GetValue(property.Name + "_delete")
                                .ConvertTo(typeof(bool), CultureInfo.CurrentCulture)).GetValueOrDefault();

                    if (file.ContentLength > 0)
                        isDeleted = false;

                    if (isDeleted)
                    {
                        propertyValue.DataBehavior = DataBehavior.Clear;
                        propertyValue.Additional = null;
                    }
                }
                else
                {
                    var value = collection.GetValue(property.Name);
                    if (value != null)
                    {
                        if (property.IsForeignKey && property.TypeInfo.IsCollection)
                        {
                            propertyValue.Values = value.AttemptedValue
                                .Split(',').OfType<object>().ToList();
                        }
                        else
                        {
                            propertyValue.Raw = value.ConvertTo(
                                property.TypeInfo.Type,
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

        public void Fill(IDictionary<string, object> item)
        {
            foreach (var property in Entity.Properties)
            {
                Values.Add(new PropertyValue(property)
                {
                    Raw = item.ContainsKey(property.Column.Undecorate()) ?
                        item[property.Column.Undecorate()] :
                        null
                });
            }
        }

        public void Fill(
            string key,
            FormCollection collection,
            HttpFileCollectionBase files,
            Func<Property, object> defaultValueResolver = null)
        {
            Fill(collection, files, defaultValueResolver);
            SetKeyValue(key);
        }

        internal void Fill(NameValueCollection request)
        {
            foreach (var property in Entity.Properties)
            {
                Values.Add(new PropertyValue(property)
                {
                    Raw = request[property.Name]
                });
            }
        }

        public void SetKeyValue(string key)
        {
            var keys = key.Split(Const.KeyColSeparator).Select(x => x.Trim()).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                Key[i].ToObject(keys[i]);
            }
        }

        public object CreateInstance()
        {
            var instance = Activator.CreateInstance(Entity.Type, null);

            foreach (var propertyValue in Values
                .Where(value =>
                    value.Raw != null &&
                    !value.Property.IsForeignKey ||
                    (value.Property.IsForeignKey && value.Property.TypeInfo.IsSystemType)))
            {
                var propertyInfo = Entity.Type.GetProperty(propertyValue.Property.Name);
                propertyInfo.SetValue(instance, propertyValue.AsObject);
            }

            return instance;
        }

        /// <summary>
        /// Get display name for entity
        /// </summary>
        /// <returns>Display name</returns>
        public override string ToString()
        {
            return new DataRow(this).ToString(Entity);
        }
    }
}