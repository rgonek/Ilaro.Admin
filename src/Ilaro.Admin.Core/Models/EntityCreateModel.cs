using System.Collections.Generic;

namespace Ilaro.Admin.Core.Models
{
    public class EntityCreateModel
    {
        public Entity Entity { get; set; }
        public IList<GroupProperties> PropertiesGroups { get; set; }
    }
}
