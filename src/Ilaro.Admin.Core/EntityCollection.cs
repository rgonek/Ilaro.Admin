using Dawn;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Ilaro.Admin.Core
{
    public sealed class EntityCollection : IEntityCollection
    {
        private readonly Dictionary<Type, Entity> _entities = new Dictionary<Type, Entity>();

        public Entity this[Type type]
        {
            get
            {
                if (_entities.ContainsKey(type) == false)
                    throw new KeyNotFoundException(type.Name);
                return _entities[type];
            }
            private set
            {
                Guard.Argument(type, nameof(type)).NotNull();
                Guard.Argument(value, nameof(value)).NotNull();

                _entities[type] = value;
            }
        }

        public Entity this[string name] => _entities.Values.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public void Add(Entity entity)
        {
            Guard.Argument(entity, nameof(entity)).NotNull();

            this[entity.Type] = entity;
        }

        public IEnumerator<Entity> GetEnumerator() => _entities.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
