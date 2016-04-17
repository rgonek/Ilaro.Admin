using System.Collections.Generic;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class GroupModel
    {
        public string Name { get; set; }

        public IList<Entity> Entities { get; set; }
    }
}
