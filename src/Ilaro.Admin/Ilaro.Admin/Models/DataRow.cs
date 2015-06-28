using System.Collections.Generic;

namespace Ilaro.Admin.Models
{
    public class DataRow
    {
        public string KeyValue { get; set; }

        public string LinkKeyValue { get; set; }

        public string DisplayName { get; set; }

        public IList<CellValue> Values { get; set; }

        public DataRow()
        {
            Values = new List<CellValue>();
        }
    }
}
