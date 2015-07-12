using System.Collections.Generic;

namespace Ilaro.Admin.Core
{
    public class RecordHierarchy
    {
        public IList<string> KeyValue { get; set; }
        public string JoinedKeyValue { get { return string.Join(Const.KeyColSeparator.ToString(), KeyValue); } }

        public string DisplayName { get; set; }

        public Entity Entity { get; set; }

        public IList<RecordHierarchy> SubRecordsHierarchies { get; set; }
    }
}