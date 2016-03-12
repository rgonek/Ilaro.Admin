using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core
{
    [DebuggerDisplay("Entity {Name}")]
    public class Entity
    {
        public Type Type { get; private set; }

        public string Name { get; private set; }

        public string TableName { get; private set; }

        public Verbose Verbose { get; private set; }

        public IList<Property> Properties { get; private set; }

        public IEnumerable<Property> FilterProperties
        {
            get
            {
                return Properties.Where(x => x.TypeInfo.IsBool);
            }
        }

        public IList<Property> Key
        {
            get
            {
                return Properties.Where(x => x.IsKey).ToList();
            }
        }

        public string JoinedKey
        {
            get { return string.Join(Const.KeyColSeparator.ToString(), Key.Select(x => x.ColumnName)); }
        }

        public string JoinedKeyWithValue
        {
            get { return string.Join(Const.KeyColSeparator.ToString(), Key.Select(x => string.Format("{0}={1}", x.Name, x.Value.AsString))); }
        }

        public string JoinedKeyValue
        {
            get { return string.Join(Const.KeyColSeparator.ToString(), Key.Select(x => x.Value.AsString)); }
        }

        public bool IsChangeEntity
        {
            get { return TypeInfo.IsChangeEntity(Type); }
        }

        public IList<GroupProperties> Groups { get; private set; }

        public IList<Property> DisplayProperties { get; private set; }

        public IEnumerable<Property> SearchProperties { get; private set; }

        public bool IsSearchActive
        {
            get { return SearchProperties.Any(); }
        }

        public Links Links { get; private set; }

        public bool CanAdd { get; private set; }

        public bool HasToStringMethod { get; private set; }

        public string RecordDisplayFormat { get; internal set; }

        private object[] Attributes
        {
            get { return Type.GetCustomAttributes(false); }
        }

        public Property this[string propertyName]
        {
            get { return Properties.FirstOrDefault(x => x.Name == propertyName); }
        }

        public Entity(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;
            Name = Type.Name;
            Verbose = new Verbose(type);

            Properties = type.GetProperties()
                .Select(x => new Property(this, x))
                .ToList();

            Links = new Links(Attributes);
            SetTableName(Attributes);

            SetSearchProperties(Attributes);

            CanAdd = true;
            if (IsChangeEntity)
            {
                CanAdd = Links.HasDelete = Links.HasEdit = false;
            }

            // check if has ToString() method
            HasToStringMethod =
                Type.GetMethod("ToString")
                .DeclaringType.Name != "Object";

            var recordDisplay = Attributes.OfType<RecordDisplayAttribute>().FirstOrDefault();
            if (recordDisplay != null)
            {
                RecordDisplayFormat = recordDisplay.DisplayFormat;
            }
        }

        private void SetTableName(object[] attributes)
        {
            var tableAttribute = attributes
                .OfType<TableAttribute>()
                .FirstOrDefault();
            if (tableAttribute != null)
            {
                SetTableName(tableAttribute.Name, tableAttribute.Schema);
            }
            else
            {
                TableName = "[" + Name.Pluralize() + "]";
            }
        }

        internal void SetTableName(string table, string schema = null)
        {
            if (!schema.IsNullOrEmpty())
            {
                TableName = "[" + schema + "].[" + table + "]";
            }
            else
            {
                TableName = "[" + table + "]";
            }
        }

        public void SetColumns()
        {
            var attributes = Type.GetCustomAttributes(false);
            SetColumns(attributes);
        }

        private void SetColumns(object[] attributes)
        {
            // if there any display properties that mean 
            // it was setted by fluent configuration 
            // and we don't want replace them
            if (!DisplayProperties.IsNullOrEmpty()) return;

            var columnsAttribute = attributes
                .OfType<ColumnsAttribute>()
                .FirstOrDefault();
            if (columnsAttribute != null)
            {
                SetColumns(columnsAttribute.Columns);
            }
            else
            {
                DisplayProperties = this.GetDefaultDisplayProperties().ToList();
            }
        }

        internal void SetColumns(IEnumerable<string> properties)
        {
            DisplayProperties = new List<Property>();
            foreach (var column in properties)
            {
                DisplayProperties
                    .Add(Properties.FirstOrDefault(x => x.Name == column));
            }
        }

        private void SetSearchProperties(IEnumerable<object> attributes)
        {
            var searchAttribute = attributes
                .OfType<SearchAttribute>()
                .FirstOrDefault();
            if (searchAttribute != null)
            {
                SetSearchProperties(searchAttribute.Columns);
            }
            else
            {
                SearchProperties = this.GetDefaultSearchProperties();
            }
        }

        internal void SetSearchProperties(IEnumerable<string> properties)
        {
            SearchProperties = Properties.Where(x => properties.Contains(x.Name));
        }

        public void PrepareGroups()
        {
            if (!Groups.IsNullOrEmpty())
            {
                return;
            }

            SetGroups(Attributes);
        }

        private void SetGroups(IEnumerable<object> attributes)
        {
            var groupsAttribute =
                attributes.OfType<GroupsAttribute>().FirstOrDefault();
            Groups = this.PrepareGroups(
                groupsAttribute != null ?
                    groupsAttribute.Groups :
                    new List<string>());
        }

        internal void AddGroup(
            string group,
            bool isCollapsed,
            IEnumerable<string> propertiesNames)
        {
            if (Groups == null)
            {
                Groups = new List<GroupProperties>();
            }

            Groups.Add(new GroupProperties
            {
                GroupName = group,
                Properties = Properties.Where(x => propertiesNames.Contains(x.Name)),
                IsCollapsed = isCollapsed
            });
        }

        internal void SetKey(string propertyName)
        {
            var property = this[propertyName];
            if (property == null)
                throw new NullReferenceException(
                    "Not found property with gived name {0}.".Fill(propertyName));

            property.IsKey = true;
        }

        public void Fill(FormCollection collection, HttpFileCollectionBase files, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            foreach (var property in Properties)
            {
                if (property.TypeInfo.IsFile)
                {
                    var file = files[property.Name];
                    property.Value.Raw = file;
                    if (property.TypeInfo.IsFileStoredInDb == false &&
                        property.FileOptions.NameCreation == NameCreation.UserInput)
                    {
                        var providedName = (string)collection.GetValue(property.Name)
                            .ConvertTo(typeof(string), culture);
                        property.Value.Additional = providedName;
                    }
                    var isDeleted =
                        ((bool?)
                            collection.GetValue(property.Name + "_delete")
                                .ConvertTo(typeof(bool), culture)).GetValueOrDefault();

                    if (file.ContentLength > 0)
                        isDeleted = false;

                    if (isDeleted)
                    {
                        property.Value.Raw = DataBehavior.Clear;
                        property.Value.Additional = null;
                    }
                }
                else
                {
                    var value = collection.GetValue(property.Name);
                    if (value != null)
                    {
                        if (property.IsForeignKey && property.TypeInfo.IsCollection)
                        {
                            property.Value.Values = value.AttemptedValue
                                .Split(',').OfType<object>().ToList();
                        }
                        else
                        {
                            property.Value.Raw = value.ConvertTo(
                                property.TypeInfo.Type,
                                culture);
                        }
                    }

                    if (property.Value.DefaultValue is DefaultValueBehavior ||
                        (property.Value.Raw == null && property.Value.DefaultValue != null))
                    {
                        property.Value.Raw = property.Value.DefaultValue;
                    }
                }
            }
        }

        public void Fill(string key, FormCollection collection, HttpFileCollectionBase files)
        {
            SetKeyValue(key);
            Fill(collection, files);
        }

        internal void Fill(HttpRequestBase Request)
        {
            foreach (var property in Properties)
            {
                property.Value.Raw = Request[property.Name];
            }
        }

        public void SetKeyValue(string key)
        {
            var keys = key.Split(Const.KeyColSeparator).Select(x => x.Trim()).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                Key[i].Value.ToObject(keys[i]);
            }
        }

        public IList<string> GetColumns()
        {
            var properties = new List<Property>();
            properties.AddRange(Key);
            properties.AddRange(DisplayProperties);

            return properties
                .Select(x => x.ColumnName)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Get display name for entity
        /// </summary>
        /// <param name="row">Instance value</param>
        /// <returns>Display name</returns>
        public string ToString(DataRow row)
        {
            // check if has to string attribute
            if (!RecordDisplayFormat.IsNullOrEmpty())
            {
                var result = RecordDisplayFormat;
                foreach (var cellValue in row.Values)
                {
                    result = result.Replace("{" + cellValue.Property.Name + "}", cellValue.AsString);
                }

                return result;
            }
            // if not check if has ToString() method
            if (HasToStringMethod)
            {
                var methodInfo = Type.GetMethod("ToString");
                var instance = Activator.CreateInstance(Type, null);

                foreach (var cellValue in row.Values
                    .Where(x =>
                        !x.Property.IsForeignKey ||
                        (x.Property.IsForeignKey && x.Property.TypeInfo.IsSystemType)))
                {
                    var propertyInfo = Type.GetProperty(cellValue.Property.Name);
                    propertyInfo.SetValue(instance, cellValue.Raw);
                }

                var result = methodInfo.Invoke(instance, null);

                return result.ToStringSafe();
            }
            // if not get first matching property
            // %Name%, %Title%, %Description%, %Value%
            // if not found any property use KeyValue
            var possibleNames = new List<string> { "name", "title", "description", "value" };
            var value = String.Empty;
            foreach (var possibleName in possibleNames)
            {
                var cell = row.Values
                    .FirstOrDefault(x =>
                        x.Property.Name.ToLower().Contains(possibleName));
                if (cell != null)
                {
                    value = cell.AsString;
                    break;
                }
            }

            if (value.IsNullOrEmpty())
            {
                return "#" + row.JoinedKeyValue;
            }

            return value;
        }

        public override string ToString()
        {
            return ToString(new DataRow(this));
        }

        public IEnumerable<Property> GetForeignsForUpdate()
        {
            return Properties
                .Where(x =>
                    x.IsForeignKey &&
                    x.TypeInfo.IsCollection &&
                    x.ForeignEntity != null);
        }

        public object CreateIntance()
        {
            var instance = Activator.CreateInstance(Type, null);

            foreach (var property in Properties
                .Where(x =>
                    !x.IsForeignKey ||
                    (x.IsForeignKey && x.TypeInfo.IsSystemType)))
            {
                var propertyInfo = Type.GetProperty(property.Name);
                propertyInfo.SetValue(instance, property.Value.AsObject);
            }

            return instance;
        }
    }
}
