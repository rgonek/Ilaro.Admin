using System.Collections.Generic;

namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordComparer
    {
        void SkipNotChangedProperties(
            EntityRecord entityRecord,
            IDictionary<string, object> existingRecord);
    }
}