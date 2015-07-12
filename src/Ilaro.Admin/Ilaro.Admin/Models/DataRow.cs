using System;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Models
{
    public class DataRow
    {
        public IList<string> KeyValue { get; set; }
        public string JoinedKeyValue { get { return string.Join(Const.KeyColSeparator.ToString(), KeyValue); } }

        public IList<string> LinkKeyValue { get; set; }
        public string JoinedLinkKeyValue { get { return string.Join(Const.KeyColSeparator.ToString(), LinkKeyValue); } }

        public string DisplayName { get; set; }
        public IList<CellValue> Values { get; set; }

        private DataRow()
        {
            Values = new List<CellValue>();
            KeyValue = new List<string>();
            LinkKeyValue = new List<string>();
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
                KeyValue.Add(recordDict[prefix + key.ColumnName.Undecorate()].ToStringSafe());
            }
            foreach (var linkKey in entity.LinkKey)
            {
                LinkKeyValue.Add(recordDict[prefix + linkKey.ColumnName.Undecorate()].ToStringSafe());
            }

            foreach (var property in entity.DisplayProperties)
            {
                Values.Add(new CellValue
                {
                    Raw = recordDict[prefix + property.ColumnName.Undecorate()],
                    Property = property
                });
            }
        }
    }
}
