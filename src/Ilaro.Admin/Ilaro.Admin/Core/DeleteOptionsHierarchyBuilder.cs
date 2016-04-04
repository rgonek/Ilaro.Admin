using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public class DeleteOptionsHierarchyBuilder
    {
        public static IList<PropertyDeleteOption> GetHierarchy(
            Entity entity,
            bool collapsed = false,
            string hierarchyNamePrefix = null,
            int level = 0)
        {
            if (hierarchyNamePrefix.HasValue())
                hierarchyNamePrefix += "-";
            var properties = new List<PropertyDeleteOption>();
            foreach (var property in entity.Properties
                       .WhereOneToMany())
            {
                var hierarchyName = hierarchyNamePrefix + property.ForeignEntity.Name;
                var visible = property.CascadeOption == CascadeOption.AskUser ||
                    property.CascadeOption == CascadeOption.Delete;
                properties.Add(new PropertyDeleteOption
                {
                    EntityName = property.ForeignEntity.Name,
                    HierarchyName = hierarchyName,
                    DeleteOption = property.CascadeOption,
                    ShowOptions = property.CascadeOption == CascadeOption.AskUser,
                    Collapsed = collapsed,
                    Level = level,
                    Visible = visible
                });

                if (visible)
                {
                    properties.AddRange(GetHierarchy(
                        property.ForeignEntity,
                        collapsed ?
                            true :
                            property.CascadeOption != CascadeOption.Delete,
                        hierarchyName,
                        ++level));
                }
            }

            return properties;
        }

        public static IList<PropertyDeleteOption> Merge(
            Entity entity,
            IList<PropertyDeleteOption> propertiesDeleteOptions)
        {
            var hierarchy = GetHierarchy(entity);
            foreach (var deleteOption in hierarchy
                .Where(x => x.DeleteOption == CascadeOption.AskUser))
            {
                deleteOption.DeleteOption = propertiesDeleteOptions
                    .FirstOrDefault(x => x.HierarchyName == deleteOption.HierarchyName)
                    .DeleteOption;
            }

            return hierarchy;
        }
    }
}