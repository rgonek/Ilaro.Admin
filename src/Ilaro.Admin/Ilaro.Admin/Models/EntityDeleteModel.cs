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
        public bool AssumableDeleteHierarchyWarning { get; }

        public EntityDeleteModel()
        {
        }

        public EntityDeleteModel(IList<PropertyDeleteOption> deleteOptions)
        {
            DisplayRecordHierarchy = deleteOptions.Any();
            AssumableDeleteHierarchyWarning = deleteOptions.Any(x => x.ShowOptions);

            if (deleteOptions.Any(x => x.DeleteOption == CascadeOption.AskUser))
                PropertiesDeleteOptions = deleteOptions.Where(x => x.Visible).ToList();
            else
                PropertiesDeleteOptions = new List<PropertyDeleteOption>();
        }
    }
}
