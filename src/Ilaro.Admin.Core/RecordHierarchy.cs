﻿using System.Collections.Generic;

namespace Ilaro.Admin.Core
{
    public class RecordHierarchy
    {
        public string JoinedKeysValues { get; set; }
        public string DisplayName { get; set; }
        public Entity Entity { get; set; }
        public IList<RecordHierarchy> SubRecordsHierarchies { get; set; }
    }
}