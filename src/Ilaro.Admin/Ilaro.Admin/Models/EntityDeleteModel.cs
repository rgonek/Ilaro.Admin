using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class EntityDeleteModel
    {
        public Entity Entity { get; set; }

        public string EntityName { get; set; }

        public string Key { get; set; }

        public IList<PropertyDeleteOption> PropertiesDeleteOptions { get; set; }

        public RecordHierarchy RecordHierarchy { get; set; }

        public EntityDeleteModel()
        {
        }

        public EntityDeleteModel(Entity entity)
        {
            Entity = entity;
            PropertiesDeleteOptions =
                entity.Properties
                    .Where(x =>
                        x.IsForeignKey &&
                        x.DeleteOption == DeleteOption.AskUser)
                    .Select(x =>
                        new PropertyDeleteOption
                        {
                            PropertyName = x.ForeignEntity.Name
                        })
                    .ToList();
        }
    }
}
