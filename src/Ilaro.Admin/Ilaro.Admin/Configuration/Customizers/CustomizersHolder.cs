using System;
using System.Collections.Generic;
using System.Reflection;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using System.Linq;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Configuration.Customizers
{
    internal class CustomizersHolder : ICustomizersHolder
    {
        public Type Type { get; private set; }
        private ClassCustomizerHolder _classCustomizer { get; } = new ClassCustomizerHolder();
        private IDictionary<MemberInfo, PropertyCustomizerHolder> _propertyCustomizers { get; } =
            new Dictionary<MemberInfo, PropertyCustomizerHolder>();

        public CustomizersHolder(Type type)
        {
            Type = type;
            foreach (var member in Type.GetProperties())
            {
                _propertyCustomizers[member] = new PropertyCustomizerHolder();
            }
        }

        public void DisplayProperties(IEnumerable<MemberInfo> displayProperties)
        {
            foreach (var displayColumn in displayProperties)
            {
                GetPropertyCustomizer(displayColumn).IsVisible = true;
            }
        }

        public void DisplayFormat(string displayFormat)
        {
            _classCustomizer.DisplayFormat = displayFormat;
        }

        public void Link(
            string display = null,
            string edit = null,
            string delete = null)
        {
            _classCustomizer.DisplayLink = display;
            _classCustomizer.EditLink = edit;
            _classCustomizer.DeleteLink = delete;
        }

        public void Group(string group)
        {
            _classCustomizer.Group = group;
        }

        public void SearchProperties(IEnumerable<MemberInfo> searchProperties)
        {
            foreach (var searchProperty in searchProperties)
            {
                GetPropertyCustomizer(searchProperty).IsSearchable = true;
            }
        }

        public void Table(string tableName, string schema = null)
        {
            _classCustomizer.Table = tableName;
            _classCustomizer.Schema = schema;
        }

        public void Id(IEnumerable<MemberInfo> idProperties)
        {
            foreach (var idProperty in idProperties)
            {
                GetPropertyCustomizer(idProperty).IsKey = true;
            }
        }

        public void Display(string singular, string plural)
        {
            _classCustomizer.NameSingular = singular;
            _classCustomizer.NamePlural = plural;
        }

        public void PropertyGroup(
            string groupName,
            bool isCollapsed,
            IEnumerable<MemberInfo> properties)
        {
            _classCustomizer.PropertiesGroups[groupName] = isCollapsed;
            foreach (var property in properties)
            {
                GetPropertyCustomizer(property).Group = groupName;
            }
        }

        public void Editable(bool allowEdit = true)
        {
            _classCustomizer.AllowEdit = allowEdit;
        }

        public void Deletable(bool allowDelete = true)
        {
            _classCustomizer.AllowDelete = allowDelete;
        }

        public void SoftDelete()
        {
            _classCustomizer.SoftDeleteEnabled = true;
        }

        public void Property(MemberInfo memberOf, Action<IPropertyCustomizer> customizer)
        {
            customizer(new PropertyCustomizer(GetPropertyCustomizer(memberOf)));
        }

        internal PropertyCustomizerHolder GetPropertyCustomizer(MemberInfo memberInfo)
        {
            var propertyInfo = (PropertyInfo)memberInfo;
            if (_propertyCustomizers.ContainsKey(propertyInfo) == false)
            {
                _propertyCustomizers[propertyInfo] = new PropertyCustomizerHolder();
            }
            return _propertyCustomizers[propertyInfo];
        }

        public void CustomizeEntity(Entity entity, IIlaroAdmin admin)
        {
            if (entity.IsChangeEntity)
            {
                _classCustomizer.AllowEdit = false;
                _classCustomizer.AllowDelete = false;
            }

            entity.SetTableName(
                _classCustomizer.Table.GetValueOrDefault(entity.Name.Pluralize()),
                _classCustomizer.Schema);

            if (_classCustomizer.NameSingular.HasValue())
                entity.Verbose.Singular = _classCustomizer.NameSingular;
            if (_classCustomizer.NamePlural.HasValue())
                entity.Verbose.Plural = _classCustomizer.NamePlural;
            if (_classCustomizer.Group.HasValue())
                entity.Verbose.Group = _classCustomizer.Group;

            entity.RecordDisplayFormat = _classCustomizer.DisplayFormat;

            entity.SoftDeleteEnabled = _classCustomizer.SoftDeleteEnabled;

            entity.Links.Display = _classCustomizer.DisplayLink;
            entity.Links.Edit = _classCustomizer.EditLink;
            entity.Links.Delete = _classCustomizer.DeleteLink;
            if (_classCustomizer.AllowEdit.HasValue)
                entity.AllowEdit = _classCustomizer.AllowEdit.Value;
            if (_classCustomizer.AllowDelete.HasValue)
                entity.AllowDelete = _classCustomizer.AllowDelete.Value;

            if (_classCustomizer.PropertiesGroups.IsNullOrEmpty())
            {
                entity.Groups.Add(new GroupProperties
                {
                    IsCollapsed = false,
                    //Properties = entity.Properties
                });
            }
            else
            {
                foreach (var group in _classCustomizer.PropertiesGroups)
                {
                    entity.Groups.Add(new GroupProperties
                    {
                        GroupName = group.Key,
                        IsCollapsed = group.Value,
                        //Properties = entity.Properties.Where(x => x.Group == group.Key)
                    });
                }
            }

            SetDefaultId(entity);
            SetDefaultDisplayProperties(entity);
            SetDefaultSearchProperties(entity);

            foreach (var customizerPair in _propertyCustomizers)
            {
                var propertyCustomizer = customizerPair.Value;
                var property = entity[customizerPair.Key.Name];

                property.IsKey = propertyCustomizer.IsKey.GetValueOrDefault(false);
                if (propertyCustomizer.IsForeignKey)
                    property.SetForeignKey(propertyCustomizer.ForeignKey);
            }
        }

        public void CustomizeProperties(Entity entity, IIlaroAdmin admin)
        {
            SetForeignKeysReferences(entity, admin);

            foreach (var customizerPair in _propertyCustomizers)
            {
                var propertyCustomizer = customizerPair.Value;
                var property = entity[customizerPair.Key.Name];

                if (propertyCustomizer.IsVisible.HasValue)
                    property.IsVisible = propertyCustomizer.IsVisible.Value;

                if (propertyCustomizer.IsSearchable.HasValue)
                    property.IsSearchable = propertyCustomizer.IsSearchable.Value;

                if (propertyCustomizer.Column.HasValue())
                    property.Column = propertyCustomizer.Column;

                if (propertyCustomizer.DisplayName.HasValue())
                    property.Display = propertyCustomizer.DisplayName;

                if (propertyCustomizer.Description.HasValue())
                    property.Description = propertyCustomizer.Description;

                property.CascadeOption = propertyCustomizer.CascadeOption
                    .GetValueOrDefault(CascadeOption.Nothing);
                if (propertyCustomizer.DataType.HasValue)
                {
                    property.TypeInfo.SourceDataType = propertyCustomizer.SourceDataType;
                    property.TypeInfo.DataType = propertyCustomizer.DataType.Value;
                    property.TypeInfo.EnumType = propertyCustomizer.EnumType;
                }
                if (propertyCustomizer.FileOptions != null)
                    property.FileOptions = propertyCustomizer.FileOptions;
                if (propertyCustomizer.Group != null)
                    property.Group = propertyCustomizer.Group;
                property.OnCreateDefaultValue = propertyCustomizer.OnCreateDefaultValue;
                property.OnUpdateDefaultValue = propertyCustomizer.OnUpdateDefaultValue;
                property.OnDeleteDefaultValue = propertyCustomizer.OnDeleteDefaultValue;
                property.Format = propertyCustomizer.Format;
                property.IsRequired = propertyCustomizer.IsRequired;
                if (propertyCustomizer.Validators.IsNullOrEmpty() == false)
                    property.Validators = propertyCustomizer.Validators.ToList();

                if (propertyCustomizer.DisplayTemplate.IsNullOrEmpty() == false)
                {
                    property.Template.Display = propertyCustomizer.DisplayTemplate;
                }
                else
                {
                    property.Template.Display =
                        TemplateUtil.GetDisplayTemplate(property.TypeInfo, property.IsForeignKey);
                }
                if (propertyCustomizer.EditorTemplate.IsNullOrEmpty() == false)
                {
                    property.Template.Editor = propertyCustomizer.EditorTemplate;
                }
                else
                {
                    property.Template.Editor =
                        TemplateUtil.GetEditorTemplate(property.TypeInfo, property.IsForeignKey);
                }
            }

            if (entity.SoftDeleteEnabled && entity.Properties.All(x => x.OnDeleteDefaultValue == null))
            {
                throw new InvalidOperationException("When soft delete for entity is enabled some of the property must have delete default value");
            }
        }

        private void SetForeignKeysReferences(Entity entity, IIlaroAdmin admin)
        {
            foreach (var property in entity.Properties.Where(x => x.IsForeignKey))
            {
                property.ForeignEntity = admin.GetEntity(property.ForeignEntityName);

                if (property.ReferencePropertyName.IsNullOrEmpty() == false)
                {
                    property.ReferenceProperty = entity[property.ReferencePropertyName];
                    if (property.ReferenceProperty != null)
                    {
                        property.ReferenceProperty.IsForeignKey = true;
                        property.ReferenceProperty.ForeignEntity = property.ForeignEntity;
                        property.ReferenceProperty.ReferenceProperty = property;
                    }
                    else if (property.TypeInfo.IsSystemType == false)
                    {
                        property.TypeInfo.Type = property.ForeignEntity == null ?
                            // by default foreign property is int
                            property.TypeInfo.Type = typeof(int) :
                            property.ForeignEntity.Key.FirstOrDefault().TypeInfo.Type;
                    }
                }
            }
        }

        private void SetDefaultId(Entity entity)
        {
            if (_propertyCustomizers.Any(x => x.Value.IsKey == true) == false)
            {
                var key = entity.Properties.FirstOrDefault(x =>
                    x.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                if (key == null)
                {
                    key = entity.Properties.FirstOrDefault(x =>
                        x.Name.Equals(entity.Name + "id", StringComparison.CurrentCultureIgnoreCase));
                }

                if (key != null)
                {
                    Id(new[] { key.PropertyInfo });
                }
            }
        }

        private void SetDefaultDisplayProperties(Entity entity)
        {
            if (_propertyCustomizers.Any(x => x.Value.IsVisible.HasValue) == false)
            {
                DisplayProperties(entity.GetDefaultDisplayProperties().Select(x => x.PropertyInfo));
            }
        }

        private void SetDefaultSearchProperties(Entity entity)
        {
            if (_propertyCustomizers.Any(x => x.Value.IsSearchable.HasValue) == false)
            {
                SearchProperties(entity.GetDefaultSearchProperties().Select(x => x.PropertyInfo));
            }
        }
    }
}