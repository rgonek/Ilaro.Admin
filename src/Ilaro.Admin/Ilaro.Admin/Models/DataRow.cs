using System;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using System.Linq;

namespace Ilaro.Admin.Models
{
    public class DataRow
    {
        public IList<string> KeyValue { get; set; }
        public string JoinedKeyValue { get { return string.Join(Const.KeyColSeparator.ToString(), KeyValue); } }

        public string DisplayName { get; set; }
        public IList<CellValue> Values { get; set; }

        private DataRow()
        {
            Values = new List<CellValue>();
            KeyValue = new List<string>();
        }

        public DataRow(EntityRecord entityRecord)
            : this()
        {
            foreach (var propertyValue in entityRecord.Values)
            {
                Values.Add(new CellValue
                {
                    Raw = propertyValue.Raw,
                    Property = propertyValue.Property
                });
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
            foreach (var key in entity.Key)
            {
                KeyValue.Add(recordDict[prefix + key.Column.Undecorate()].ToStringSafe());
            }

            foreach (var property in entity.DisplayProperties)
            {
                Values.Add(new CellValue
                {
                    Raw = recordDict[prefix + property.Column.Undecorate()],
                    Property = property
                });
            }
        }

        public string ToString(Entity entity)
        {
            // check if has to string attribute
            if (entity.RecordDisplayFormat.HasValue())
            {
                var result = entity.RecordDisplayFormat;
                foreach (var cellValue in Values)
                {
                    result = result.Replace("{" + cellValue.Property.Name + "}", cellValue.AsString);
                }

                return result;
            }
            // if not check if has ToString() method
            if (entity.HasToStringMethod)
            {
                var methodInfo = entity.Type.GetMethod("ToString");
                var instance = Activator.CreateInstance(entity.Type, null);

                foreach (var cellValue in Values
                    .Where(x =>
                        !x.Property.IsForeignKey ||
                        (x.Property.IsForeignKey && x.Property.TypeInfo.IsSystemType)))
                {
                    var propertyInfo = entity.Type.GetProperty(cellValue.Property.Name);
                    propertyInfo.SetValue(instance, cellValue.AsObject);
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
