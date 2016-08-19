using System.Collections.Generic;

namespace Ilaro.Admin.Core.Models
{
    public class EntityEditModel
    {
        public Entity Entity { get; set; }
        public EntityRecord Record { get; set; }
        public IList<GroupProperties> PropertiesGroups { get; set; }
        public object ConcurrencyCheck { get; set; }
    }
}
