using System;
using System.Collections.Generic;
using System.Reflection;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using Resources;

namespace Ilaro.Admin.Configuration.Customizers
{
    public class CustomizersHolder : ICustomizersHolder
    {
        public Type Type { get; private set; }
        public ClassCustomizerHolder ClassCustomizer { get; } = new ClassCustomizerHolder();
        public IDictionary<MemberInfo, PropertyCustomizerHolder> PropertyCustomizers { get; } =
            new Dictionary<MemberInfo, PropertyCustomizerHolder>();

        public CustomizersHolder(Type type)
        {
            Type = type;
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
            ClassCustomizer.DisplayFormat = displayFormat;
        }

        public void DisplayLink(string displayLink)
        {
            ClassCustomizer.DisplayLink = displayLink;
        }

        public void EditLink(string editLink)
        {
            ClassCustomizer.EditLink = editLink;
        }

        public void DeleteLink(string deleteLink)
        {
            ClassCustomizer.DeleteLink = deleteLink;
        }

        public void Group(string group)
        {
            ClassCustomizer.Group = group;
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
            ClassCustomizer.Table = tableName;
            ClassCustomizer.Schema = schema;
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
            ClassCustomizer.NameSingular = singular;
            ClassCustomizer.NamePlural = plural;
        }

        public void PropertyGroup(
            string groupName,
            bool isCollapsed,
            IEnumerable<MemberInfo> properties)
        {
            ClassCustomizer.Groups[groupName] = isCollapsed;
            foreach (var property in properties)
            {
                GetPropertyCustomizer(property).Group = groupName;
            }
        }

        public void Property(MemberInfo memberOf, Action<IPropertyCustomizer> customizer)
        {
            customizer(new PropertyCustomizer(GetPropertyCustomizer(memberOf)));
        }

        internal PropertyCustomizerHolder GetPropertyCustomizer(MemberInfo memberInfo)
        {
            var propertyInfo = (PropertyInfo)memberInfo;
            if (PropertyCustomizers.ContainsKey(propertyInfo) == false)
            {
                PropertyCustomizers[propertyInfo] = new PropertyCustomizerHolder();
            }
            return PropertyCustomizers[propertyInfo];
        }

        public void CustomizeEntity(Entity entity)
        {
            entity.SetTableName(
                ClassCustomizer.Table.GetValueOrDefault(entity.Name.Pluralize()),
                ClassCustomizer.Schema);

            entity.Verbose.Singular = ClassCustomizer.NameSingular
                .GetValueOrDefault(Type.Name.SplitCamelCase());
            entity.Verbose.Plural = ClassCustomizer.NamePlural
                .GetValueOrDefault(entity.Verbose.Singular.Pluralize().SplitCamelCase());
            entity.Verbose.Group = ClassCustomizer.Group
                .GetValueOrDefault(IlaroAdminResources.Others);

            entity.RecordDisplayFormat = ClassCustomizer.DisplayFormat;

            entity.Links.Display = ClassCustomizer.DisplayLink;
            entity.Links.Edit = ClassCustomizer.EditLink;
            entity.Links.Delete = ClassCustomizer.DeleteLink;

            foreach (var customizerPair in PropertyCustomizers)
            {
                var propertyCustomizer = customizerPair.Value;
                var property = entity[customizerPair.Key.Name];

                property.IsVisible = propertyCustomizer.IsVisible
                    .GetValueOrDefault(true);
                property.IsSearchable = propertyCustomizer.IsSearchable
                    .GetValueOrDefault(true);
                property.IsKey = propertyCustomizer.IsKey.GetValueOrDefault(false);
                property.ColumnName = propertyCustomizer.Column
                    .GetValueOrDefault(property.Name);
                property.DisplayName = propertyCustomizer.DisplayName
                    .GetValueOrDefault(property.Name.SplitCamelCase());
                property.Description = propertyCustomizer.Description;
                property.DeleteOption = propertyCustomizer.DeleteOption
                    .GetValueOrDefault(DeleteOption.AskUser);
                property.Template.Display = propertyCustomizer.DisplayTemplate
                    .GetValueOrDefault(property.Template.Display);
                property.Template.Editor = propertyCustomizer.EditorTemplate
                    .GetValueOrDefault(property.Template.Editor);
                property.TypeInfo.DataType = propertyCustomizer.DataType
                    .GetValueOrDefault(property.TypeInfo.DataType);
                property.FileOptions = propertyCustomizer.FileOptions;
                property.GroupName = propertyCustomizer.Group;
                property.Value.DefaultValue = propertyCustomizer.DefaultValue;
                property.Format = propertyCustomizer.Format;
                property.IsRequired = propertyCustomizer.IsRequired;
                property.RequiredErrorMessage = propertyCustomizer.RequiredErrorMessage;
                property.SetForeignKey(propertyCustomizer.ForeignKey);
            }
        }
    }
}