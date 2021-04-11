using System;

namespace Ilaro.Admin.DataAccess
{
    public class EntityMetadata
    {
        public string RecordDisplayFormat { get; internal set; }

        public bool HasToStringMethod { get; private set; }

        public Type Type { get; private set; }

        public string Table { get; internal set; }
        public object Keys { get; internal set; }
    }
}
