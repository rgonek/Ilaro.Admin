using System.Collections.Generic;

namespace Ilaro.Admin.Core.Models
{
    public class GroupModel
    {
        public string Name { get; set; }

        public IList<Entity> Entities { get; set; }
    }
}
