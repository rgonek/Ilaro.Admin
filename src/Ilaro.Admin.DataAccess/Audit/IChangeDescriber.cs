using Ilaro.Admin.Commons;
using System.Collections.Generic;

namespace Ilaro.Admin.DataAccess.Audit
{
    public interface IChangeDescriber
    {
        string UpdateChanges(EntityRecord entityRecord, IDictionary<string, object> existingRecord);

        string CreateChanges(EntityRecord entityRecord);

        string DeleteChanges(EntityRecord entityRecord, IDictionary<string, object> existingRecord);
    }
}