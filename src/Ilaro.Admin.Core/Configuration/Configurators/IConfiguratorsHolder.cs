using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ilaro.Admin.Core.Configuration.Configurators
{
    public interface IConfiguratorsHolder
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
        void Property(MemberInfo memberOf, Action<IPropertyConfigurator> customizer);
        void Editable(bool allowEdit = true);
        void Deletable(bool allowDelete = true);
        void SoftDelete();
        void ConcurrencyCheck();
        void DefaultOrder(MemberInfo memberOf, OrderDirection orderType = OrderDirection.Asc);
    }
}