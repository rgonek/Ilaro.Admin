using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IComparingRecords
    {
        void SkipNotChangedProperties(
            EntityRecord entityRecord, 
            IDictionary<string, object> existingRecord);
    }
}