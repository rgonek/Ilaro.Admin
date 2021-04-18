using System.Collections.Generic;
using Ilaro.Admin.Core.Filters;
using Ilaro.Admin.Core.Models;

namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordFetcher
    {
        EntityRecord GetEntityRecord(Entity entity, string value);
        EntityRecord GetEntityRecord(Entity entity, params object[] values);
        EntityRecord GetEntityRecord(Entity entity, IdValue idValue);

        IDictionary<string, object> GetRecord(Entity entity, string value);
        IDictionary<string, object> GetRecord(Entity entity, params object[] values);
        IDictionary<string, object> GetRecord(Entity entity, IdValue idValue);

        PagedRecords GetRecords(
            Entity entity,
            IList<BaseFilter> filters = null,
            string searchQuery = null,
            string order = null,
            string orderDirection = null,
            bool determineDisplayValue = false,
            int? page = null,
            int? take = null,
            bool loadForeignKeys = false);
    }
}