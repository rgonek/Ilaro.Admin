using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Services
{
    public interface IEntityService
    {
        object Create(Entity entity);

        bool Delete(
            Entity entity,
            string key,
            IEnumerable<PropertyDeleteOption> propertiesDeleteOptions);

        int Edit(Entity entity);

        object GetKeyValue(Entity entity, object savedItem);

        IList<GroupProperties> PrepareGroups(
            Entity entity,
            bool getKey = true,
            string key = null);

        RecordHierarchy GetRecordHierarchy(Entity entity);
    }
}