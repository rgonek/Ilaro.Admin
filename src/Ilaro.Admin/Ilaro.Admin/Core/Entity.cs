using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.ViewModels;

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

        public Property Key
        {
            get
            {
                return Properties.FirstOrDefault(x => x.IsKey);
            }
        }

        public Property LinkKey
        {
            get
            {
                return Properties.FirstOrDefault(x => x.IsLinkKey);
            }
        }

        public bool IsChangeEntity
        {
            get { return TypeInfo.IsChangeEntity(Type); }
        }

        public IList<GroupPropertiesViewModel> Groups { get; set; }

        public IList<Property> DisplayProperties { get; set; }

        public IEnumerable<Property> SearchProperties { get; set; }

        public Links Links { get; set; }

        public bool CanAdd { get; set; }

        public bool HasToStringMethod { get; set; }

        public string RecordDisplayFormat { get; set; }

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
                Key.IsLinkKey = true;
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

            Groups = new List<GroupPropertiesViewModel>();
            if (groupsNames.IsNullOrEmpty())
            {
                foreach (var group in groupsDict)
                {
                    Groups.Add(new GroupPropertiesViewModel
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

                    Groups.Add(new GroupPropertiesViewModel
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
                Groups = new List<GroupPropertiesViewModel>();
            }

            Groups.Add(new GroupPropertiesViewModel
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
                // If property is key,  
                // and I want get a key (getKey == true) && data type is string
                else if (
                    property.IsKey &&
                    property.TypeInfo.DataType == DataType.Text &&
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
    }
}
