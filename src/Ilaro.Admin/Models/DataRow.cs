using System;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using System.Linq;
using Ilaro.Admin.Core.Data;

namespace Ilaro.Admin.Models
{
    public class DataRow
    {
        public IList<string> KeyValue { get; set; }
        public string JoinedKeyValue { get { return string.Join(Const.KeyColSeparator.ToString(), KeyValue); } }

        public string DisplayName { get; set; }
        public IList<PropertyValue> Values { get; set; }

        private DataRow()
        {
            Values = new List<PropertyValue>();
            KeyValue = new List<string>();
        }

        public DataRow(EntityRecord entityRecord)
            : this()
        {
            foreach (var propertyValue in entityRecord.Values)
            {
                Values.Add(propertyValue);
            }
        }

        public DataRow(
            dynamic record,
            Entity entity,
            string prefix = null)
            : this((IDictionary<String, Object>)record, entity, prefix)
        {
        }

        public DataRow(
            IDictionary<String, object> recordDict,
            Entity entity,
            string prefix = null)
            : this()
        {
            foreach (var key in entity.Keys)
            {
                KeyValue.Add(recordDict[prefix + key.Column.Undecorate()].ToStringSafe());
            }

            foreach (var property in entity.DisplayProperties)
            {
                Values.Add(new PropertyValue(property)
                {
                    Raw = recordDict[prefix + property.Column.Undecorate()]
                });
            }
        }

        public string ToString(Entity entity)
        {
            // check if has to string attribute
            if (entity.RecordDisplayFormat.HasValue())
            {
                var result = entity.RecordDisplayFormat;
                foreach (var PropertyValue in Values)
                {
                    result = result.Replace("{" + PropertyValue.Property.Name + "}", PropertyValue.AsString);
                }

                return result;
            }
            // if not check if has ToString() method
            if (entity.HasToStringMethod)
            {
                var methodInfo = entity.Type.GetMethod("ToString");
                var instance = Activator.CreateInstance(entity.Type, null);

                foreach (var PropertyValue in Values
                    .Where(x =>
                        (x.Raw is ValueBehavior) == false &&
                        !x.Property.IsForeignKey ||
                        (x.Property.IsForeignKey && x.Property.TypeInfo.IsSystemType)))
                {
                    var propertyInfo = entity.Type.GetProperty(PropertyValue.Property.Name);
                    propertyInfo.SetValue(instance, PropertyValue.AsObject);
                }

                var result = methodInfo.Invoke(instance, null);

                return result.ToStringSafe();
            }
            // if not get first matching property
            // %Name%, %Title%, %Description%, %Value%
            // if not found any property use KeyValue
            var possibleNames = new List<string> { "name", "title", "description", "value" };
            var value = string.Empty;
            foreach (var possibleName in possibleNames)
            {
                var cell = Values
                    .FirstOrDefault(x =>
                        x.Property.Name.ToLower().Contains(possibleName));
                if (cell != null)
                {
                    value = cell.AsString;
                    break;
                }
            }

            if (value.IsNullOrEmpty())
            {
                return "#" + JoinedKeyValue;
            }

            return value;
        }
    }
}
