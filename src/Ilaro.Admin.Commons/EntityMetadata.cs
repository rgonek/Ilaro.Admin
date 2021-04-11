using System;

namespace Ilaro.Admin.Common
{
    public class EntityMetadata
    {
        public string RecordDisplayFormat { get; internal set; }

        public bool HasToStringMethod { get; private set; }

        public Type Type { get; private set; }
    }
}
