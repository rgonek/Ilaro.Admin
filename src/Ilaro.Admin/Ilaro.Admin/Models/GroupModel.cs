using Ilaro.Admin.Core;
using System.Collections.Generic;

namespace Ilaro.Admin.Models
{
    public class GroupModel
    {
        public string Name { get; set; }

        public IList<Entity> Entities { get; set; }
    }
}
