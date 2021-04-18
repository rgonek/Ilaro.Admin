using Ilaro.Admin.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace Ilaro.Admin.Core
{
    public class EntityRecord
    {
        public Entity Entity { get; }

        public IList<PropertyValue> Values { get; } = new List<PropertyValue>();

        public IEnumerable<PropertyValue> DisplayValues => Values.Where(x => x.Property.IsVisible);

        public IdValue Id => Values.Where(value => value.Property.IsKey).ToList();

        public PropertyValue ConcurrencyCheck => Values.FirstOrDefault(x => x.Property.IsConcurrencyCheck);

        public PropertyValue this[string propertyName] => Values.FirstOrDefault(x => x.Property.Name == propertyName);

        public EntityRecord(Entity entity)
        {
            Guard.Argument(entity, nameof(entity)).NotNull();

            Entity = entity;
        }

        /// <summary>
        /// Get display name for record
        /// </summary>
        public override string ToString()
        {
            // check if has to string attribute
            if (Entity.RecordDisplayFormat.HasValue())
            {
                var result = Entity.RecordDisplayFormat;
                foreach (var PropertyValue in Values)
                {
                    result = result.Replace("{" + PropertyValue.Property.Name + "}", PropertyValue.AsString);
                }

                return result;
            }
            // if not check if has ToString() method
            if (Entity.HasToStringMethod)
            {
                var methodInfo = Entity.Type.GetMethod("ToString");
                var instance = this.CreateInstance();

                var result = methodInfo.Invoke(instance, null);

                return result.ToStringSafe();
            }

            return null;
        }
    }
}