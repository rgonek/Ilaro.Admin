using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IComparingRecords
    {
        void SkipNotChangedProperties(Entity entity, IDictionary<string, object> existingRecord);
    }
}