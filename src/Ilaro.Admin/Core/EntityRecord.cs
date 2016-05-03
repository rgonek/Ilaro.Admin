using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public class EntityRecord
    {
        public Entity Entity { get; }

        public IList<PropertyValue> Values { get; } = new List<PropertyValue>();

        public IEnumerable<PropertyValue> DisplayValues
        {
            get
            {
                return Values.Where(x => x.Property.IsVisible);
            }
        }

        public IList<PropertyValue> Keys
        {
            get
            {
                return Values.Where(value => value.Property.IsKey).ToList();
            }
        }

        public string JoinedKeysWithNames
        {
            get
            {
                return string.Join(
                    Const.KeyColSeparator.ToString(),
                    Keys.Select(value => string.Format("{0}={1}", value.Property.Name, value.AsString)));
            }
        }

        public string JoinedKeysValues
        {
            get { return string.Join(Const.KeyColSeparator.ToString(), Keys.Select(x => x.AsString)); }
        }

        public PropertyValue ConcurrencyCheck
        {
            get
            {
                return Values.FirstOrDefault(x => x.Property.IsConcurrencyCheck);
            }
        }

        public PropertyValue this[string propertyName]
        {
            get { return Values.FirstOrDefault(x => x.Property.Name == propertyName); }
        }

        public EntityRecord(Entity entity)
        {
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