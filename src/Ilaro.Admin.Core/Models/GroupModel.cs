using System.Collections.Generic;

namespace Ilaro.Admin.Core.Models
{
    public class GroupModel
    {
        public string Name { get; }

        public IList<Entity> Entities { get; }

        public GroupModel(string name, List<Entity> entities)
        {
            Name = name;
            Entities = entities;
        }
    }
}
