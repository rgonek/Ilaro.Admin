using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public IList<Property> LinkKey
        {
            get
            {
                return Properties.Where(x => x.IsLinkKey).ToList();
            }
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
            SetLinkKey();

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

        public void SetLinkKey()
        {
            if (LinkKey == null && Key != null)
            {
                foreach (var key in Key)
                {
                    key.IsLinkKey = true;
                }
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
                DisplayProperties = GetDisplayProperties().ToList();
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
                // TODO: Move types to other class
                SearchProperties = Properties
                    .Where(x =>
                        !x.IsForeignKey &&
                        x.TypeInfo.IsAvailableForSearch);
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
            PrepareGroups(
                groupsAttribute != null ?
                    groupsAttribute.Groups :
                    new List<string>());
        }

        private void PrepareGroups(IList<string> groupsNames)
        {
            var groupsDict = CreateProperties()
                .GroupBy(x => x.GroupName)
                .ToDictionary(x => x.Key);

            Groups = new List<GroupProperties>();
            if (groupsNames.IsNullOrEmpty())
            {
                foreach (var group in groupsDict)
                {
                    Groups.Add(new GroupProperties
                    {
                        GroupName = group.Key,
                        Properties = group.Value.ToList()
                    });
                }
            }
            else
            {
                foreach (var groupName in groupsNames)
                {
                    var trimedGroupName = groupName.TrimEnd('*');
                    if (!groupsDict.ContainsKey(trimedGroupName)) continue;

                    var group = groupsDict[trimedGroupName];

                    Groups.Add(new GroupProperties
                    {
                        GroupName = @group.Key,
                        Properties = @group.ToList(),
                        IsCollapsed = groupName.EndsWith("*")
                    });
                }
            }
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

        public IEnumerable<Property> CreateProperties(
            bool getKey = true,
            bool getForeignCollection = true)
        {
            foreach (var property in Properties)
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
                else if (property.IsForeignKey)
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

        private IEnumerable<Property> GetDisplayProperties()
        {
            foreach (var property in Properties)
            {
                // Get all properties which is not a key and foreign key
                if (!property.IsForeignKey)
                {
                    yield return property;
                }

                else if (property.IsForeignKey)
                {
                    // If is foreign key and not have reference property
                    if (
                        property.ReferenceProperty == null &&
                        !property.TypeInfo.IsCollection)
                    {
                        yield return property;
                    }
                    // If is foreign key and have foreign key, that means, 
                    // we have two properties for one database column, 
                    // so I want only that one who is a system type
                    else if (
                        property.ReferenceProperty != null &&
                        property.TypeInfo.IsSystemType &&
                        !property.TypeInfo.IsCollection)
                    {
                        yield return property;
                    }
                }
            }
        }

        internal void SetKey(string propertyName)
        {
            var property = this[propertyName];
            if (property == null)
                throw new NullReferenceException(
                    "Not found property with gived name {0}.".Fill(propertyName));

            property.IsKey = true;
        }

        internal void SetLinkKey(string propertyName)
        {
            var property = this[propertyName];
            if (property == null)
                throw new NullReferenceException(
                    "Not found property with gived name {0}.".Fill(propertyName));

            property.IsLinkKey = true;
        }

        public void ClearPropertiesValues()
        {
            foreach (var property in Properties)
            {
                property.Value.Clear();
            }
        }

        public void Fill(FormCollection collection, HttpFileCollectionBase files)
        {
            foreach (var property in Properties)
            {
                if (property.TypeInfo.IsFile)
                {
                    var file = files[property.Name];
                    property.Value.Raw = file;
                }
                else
                {
                    var value = collection.GetValue(property.Name);
                    if (value == null)
                        continue;

                    if (property.IsForeignKey && property.TypeInfo.IsCollection)
                    {
                        property.Value.Values = value.AttemptedValue
                            .Split(",".ToCharArray()).OfType<object>().ToList();
                    }
                    else
                    {
                        property.Value.Raw = value.ConvertTo(
                            property.TypeInfo.Type,
                            CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        public void Fill(string key, FormCollection collection, HttpFileCollectionBase files)
        {
            var keys = key.Split(Const.KeyColSeparator).Select(x => x.Trim()).ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                Key[i].Value.ToObject(keys[i]);
            }
            Fill(collection, files);
        }

        public IList<string> GetColumns()
        {
            var properties = new List<Property>();
            properties.AddRange(Key);
            properties.AddRange(LinkKey);
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

        public IEnumerable<Property> GetForeignsForUpdate()
        {
            return Properties
                .Where(x =>
                    x.IsForeignKey &&
                    x.TypeInfo.IsCollection &&
                    x.ForeignEntity != null);
        }
    }
}
