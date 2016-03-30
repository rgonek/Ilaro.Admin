using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IDescribingChanges
    {
        string UpdateChanges(EntityRecord entityRecord, IDictionary<string, object> existingRecord);

        string CreateChanges(EntityRecord entityRecord);

        string DeleteChanges(EntityRecord entityRecord, IDictionary<string, object> existingRecord);
    }
}