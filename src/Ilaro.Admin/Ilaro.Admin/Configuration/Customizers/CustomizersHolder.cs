using System;
using System.Collections.Generic;
using System.Reflection;
using Ilaro.Admin.Core;

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

        public void Table(string tableName)
        {
            ClassCustomizer.Table = tableName;
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

        public void PropertyGroup(string groupName, bool isCollapsed, IEnumerable<MemberInfo> memberInfos)
        {
            ClassCustomizer.Groups[groupName] = isCollapsed;
        }

        public void Property(MemberInfo memberOf, Action<IPropertyCustomizer> customizer)
        {
            customizer(new PropertyCustomizer(GetPropertyCustomizer(memberOf)));
        }

        private PropertyCustomizerHolder GetPropertyCustomizer(MemberInfo memberInfo)
        {
            if (PropertyCustomizers.ContainsKey(memberInfo) == false)
            {
                PropertyCustomizers[memberInfo] = new PropertyCustomizerHolder();
            }
            return PropertyCustomizers[memberInfo];
        }

        public void CustomizeEntity(Entity entity)
        {
            entity.SetTableName(ClassCustomizer.Table);
            entity.Verbose.Singular = ClassCustomizer.NameSingular;
            entity.Verbose.Plural = ClassCustomizer.NamePlural;
        }
    }
}