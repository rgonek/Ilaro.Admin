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
                var visible = property.ForeignDeleteOption == DeleteOption.AskUser ||
                    property.ForeignDeleteOption == DeleteOption.CascadeDelete;
                properties.Add(new PropertyDeleteOption
                {
                    EntityName = property.ForeignEntity.Name,
                    HierarchyName = hierarchyName,
                    DeleteOption = property.ForeignDeleteOption,
                    ShowOptions = property.ForeignDeleteOption == DeleteOption.AskUser,
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
                            property.ForeignDeleteOption != DeleteOption.CascadeDelete,
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
                .Where(x => x.DeleteOption == DeleteOption.AskUser))
            {
                deleteOption.DeleteOption = propertiesDeleteOptions
                    .FirstOrDefault(x => x.HierarchyName == deleteOption.HierarchyName)
                    .DeleteOption;
            }

            return hierarchy;
        }
    }
}