using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Services
{
    public interface IEntityService
    {
        string Create(Entity entity);

        bool Edit(Entity entity);

        bool Delete(
            Entity entity,
            string key,
            IEnumerable<PropertyDeleteOption> options);

        IList<GroupProperties> PrepareGroups(
            Entity entity,
            bool getKey = true,
            string key = null);
    }
}