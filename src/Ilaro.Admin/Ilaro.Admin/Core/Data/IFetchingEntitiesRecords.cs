using System.Collections.Generic;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core.Data
{
    public interface IFetchingEntitiesRecords
    {
        Entity GetEntityWithData(string entityName, string key);

        object GetRecord(Entity entity, object key);

        /// <summary>
        /// Get list of records for certain page
        /// </summary>
        PagedRecords GetRecords(
            Entity entity,
            int page,
            int take,
            IList<IEntityFilter> filters,
            string searchQuery,
            string order,
            string orderDirection);

        IList<DataRow> GetRecords(
            Entity entity,
            IList<IEntityFilter> filters = null,
            string searchQuery = null,
            string order = null,
            string orderDirection = null,
            bool determineDisplayValue = false);

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