using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class EntityDeleteModel
    {
        public EntityRecord EntityRecord { get; set; }

        public string EntityName { get; set; }

        public string Key { get; set; }

        public IList<PropertyDeleteOption> PropertiesDeleteOptions { get; set; }

        public RecordHierarchy RecordHierarchy { get; set; }

        public EntityDeleteModel()
        {
        }

        public EntityDeleteModel(EntityRecord entityRecord)
        {
            EntityRecord = entityRecord;
            PropertiesDeleteOptions =
                entityRecord.Entity.Properties
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
