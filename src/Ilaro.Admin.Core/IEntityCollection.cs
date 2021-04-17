using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core
{
    public interface IEntityCollection : IEnumerable<Entity>
    {
        Entity this[Type type] { get; }

        Entity this[string name] { get; }

        void Add(Entity entity);
    }
}
