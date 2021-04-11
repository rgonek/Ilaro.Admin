using System.Collections.Generic;

namespace Ilaro.Admin.Core
{
    public class EntityHierarchy
    {
        public Entity Entity { get; set; }

        public string Alias { get; set; }

        public IList<EntityHierarchy> SubHierarchies { get; set; }

        public EntityHierarchy ParentHierarchy { get; set; }
    }
}