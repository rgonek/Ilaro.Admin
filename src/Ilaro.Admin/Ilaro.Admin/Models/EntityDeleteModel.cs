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

        public bool DisplayRecordHierarchy { get; }

        public EntityDeleteModel()
        {
        }

        public EntityDeleteModel(EntityRecord entityRecord)
        {
            EntityRecord = entityRecord;

            var deleteOptions = DeleteOptionsHierarchyBuilder
                .GetHierarchy(entityRecord.Entity, false);
            DisplayRecordHierarchy = deleteOptions.Any();

            if (deleteOptions.Any(x => x.DeleteOption == DeleteOption.AskUser))
                PropertiesDeleteOptions = deleteOptions.ToList();
            else
                PropertiesDeleteOptions = new List<PropertyDeleteOption>();
        }
    }
}
