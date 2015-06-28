using Ilaro.Admin.Core;
using System.Collections.Generic;

namespace Ilaro.Admin.Models
{
    public class EntityCreateModel
    {
        public Entity Entity { get; set; }

        public IList<GroupProperties> PropertiesGroups { get; set; }
    }
}
