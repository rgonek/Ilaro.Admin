using System.Collections.Generic;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Ilaro.Admin.Core.DataAccess
{
    public interface IEntityService
    {
        IdValue Create(
            Entity entity,
            IFormCollection collection);

        bool Edit(
            Entity entity,
            string key,
            IFormCollection collection,
            object concurrencyCheckValue = null);

        bool Delete(
            Entity entity,
            string key,
            IEnumerable<PropertyDeleteOption> options);

        IList<GroupProperties> PrepareGroups(
            EntityRecord entityRecord,
            bool getKey = true,
            string key = null);
    }
}