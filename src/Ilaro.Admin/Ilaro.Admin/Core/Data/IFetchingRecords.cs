using System.Collections.Generic;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core.Data
{
    public interface IFetchingRecords
    {
        Entity GetEntityWithData(string entityName, string key);

        object GetRecord(Entity entity, object key);

        PagedRecords GetRecords(
            Entity entity,
            IList<IEntityFilter> filters = null,
            string searchQuery = null,
            string order = null,
            string orderDirection = null,
            bool determineDisplayValue = false,
            int? page = null,
            int? take = null);

        /// <summary>
        /// Get list of changes for entity
        /// </summary>
        PagedRecords GetChangesRecords(
            Entity entityChangesFor,
            int page,
            int take,
            IList<IEntityFilter> filters,
            string searchQuery,
            string order,
            string orderDirection);
    }
}