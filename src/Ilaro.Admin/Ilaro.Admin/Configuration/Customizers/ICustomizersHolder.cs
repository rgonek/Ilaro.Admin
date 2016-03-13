using System;
using System.Collections.Generic;
using System.Reflection;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Configuration.Customizers
{
    public interface ICustomizersHolder
    {
        Type Type { get; }
        ClassCustomizerHolder ClassCustomizer { get; }
        IDictionary<MemberInfo, PropertyCustomizerHolder> PropertyCustomizers { get; }

        void Table(string tableName);
        void Id(IEnumerable<MemberInfo> idProperties);
        void DisplayProperties(IEnumerable<MemberInfo> displayProperties);
        void SearchProperties(IEnumerable<MemberInfo> searchProperties);
        void DisplayLink(string displayLink);
        void EditLink(string editLink);
        void DeleteLink(string deleteLink);
        void Group(string group);
        void DisplayFormat(string displayFormat);
        void Display(string singular, string plural);
        void PropertyGroup(string groupName, bool isCollapsed, IEnumerable<MemberInfo> memberInfos);
        void Property(MemberInfo memberOf, Action<IPropertyCustomizer> customizer);
        void CustomizeEntity(Entity entity);
    }
}