using System;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Models
{
    public class DataRow
    {
        public string KeyValue { get; set; }

        public string LinkKeyValue { get; set; }

        public string DisplayName { get; set; }

        public IList<CellValue> Values { get; set; }

        private DataRow()
        {
            Values = new List<CellValue>();
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
            KeyValue = recordDict[prefix + entity.Key.ColumnName.UnDecorate()].ToStringSafe();
            LinkKeyValue = recordDict[prefix + entity.LinkKey.ColumnName.UnDecorate()].ToStringSafe();

            foreach (var property in entity.DisplayProperties)
            {
                Values.Add(new CellValue
                {
                    Raw = recordDict[prefix + property.ColumnName.UnDecorate()],
                    Property = property
                });
            }
        }
    }
}
