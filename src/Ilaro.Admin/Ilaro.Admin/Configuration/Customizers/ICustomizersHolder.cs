using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ilaro.Admin.Configuration.Customizers
{
    public interface ICustomizersHolder
    {
        Type Type { get; }

        void Table(string tableName, string schema = null);
        void Id(IEnumerable<MemberInfo> idProperties);
        void DisplayProperties(IEnumerable<MemberInfo> displayProperties);
        void SearchProperties(IEnumerable<MemberInfo> searchProperties);
        void Link(string display = null, string edit = null, string delete = null);
        void Group(string group);
        void DisplayFormat(string displayFormat);
        void Display(string singular, string plural);
        void PropertyGroup(string groupName, bool isCollapsed, IEnumerable<MemberInfo> memberInfos);
        void Property(MemberInfo memberOf, Action<IPropertyCustomizer> customizer);
        void Editable(bool allowEdit = true);
        void Deletable(bool allowDelete = true);
        void SoftDelete();
    }
}