using System.Collections.Generic;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class EntityCreateOrEditModel
    {
        public Entity Entity { get; set; }

        public IList<GroupProperties> PropertiesGroups { get; set; }
    }
}
