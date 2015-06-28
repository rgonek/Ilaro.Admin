using System.Collections.Generic;
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
    }
}
