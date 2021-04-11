using Ilaro.Admin.Common.Extensions;
using Ilaro.Admin.DataAccess.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.DataAccess
{
    public class EntityRecord
    {
        public EntityMetadata Metadata { get; }

        public IList<PropertyValue> Values { get; } = new List<PropertyValue>();

        public IEnumerable<PropertyValue> DisplayValues
        {
            get
            {
                return Values.Where(x => x.Metadata.IsVisible);
            }
        }

        public IList<PropertyValue> Keys
        {
            get
            {
                return Values.Where(value => value.Metadata.IsKey).ToList();
            }
        }

        public string JoinedKeysWithNames
        {
            get
            {
                return string.Join(
                    Const.KeyColSeparator.ToString(),
                    Keys.Select(value => string.Format("{0}={1}", value.Metadata.Name, value.AsString)));
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
                return Values.FirstOrDefault(x => x.Metadata.IsConcurrencyCheck);
            }
        }

        public PropertyValue this[string propertyName]
        {
            get { return Values.FirstOrDefault(x => x.Metadata.Name == propertyName); }
        }

        /// <summary>
        /// Get display name for record
        /// </summary>
        public override string ToString()
        {
            // check if has to string attribute
            if (Metadata.RecordDisplayFormat.HasValue())
            {
                var result = Metadata.RecordDisplayFormat;
                foreach (var PropertyValue in Values)
                {
                    result = result.Replace("{" + PropertyValue.Metadata.Name + "}", PropertyValue.AsString);
                }

                return result;
            }
            // if not check if has ToString() method
            if (Metadata.HasToStringMethod)
            {
                var methodInfo = Metadata.Type.GetMethod("ToString");
                var instance = this.CreateInstance();

                var result = methodInfo.Invoke(instance, null);

                return result.ToStringSafe();
            }

            return null;
        }
    }
}