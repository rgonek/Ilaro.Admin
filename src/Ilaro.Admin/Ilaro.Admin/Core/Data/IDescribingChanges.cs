using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IDescribingChanges
    {
        string UpdateChanges(Entity entity, IDictionary<string, object> existingRecord);

        string CreateChanges(Entity entity);

        string DeleteChanges(Entity entity, IDictionary<string, object> existingRecord);
    }
}