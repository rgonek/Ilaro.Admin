using System.Collections.Generic;
using Ilaro.Admin.Commons.Filters;
using Ilaro.Admin.Commons.Models;

namespace Ilaro.Admin.Commons.Data
{
    public interface IFetchingRecords
    {
        EntityRecord GetEntityRecord(Entity entity, string key);
        EntityRecord GetEntityRecord(Entity entity, params string[] key);
        
        IDictionary<string, object> GetRecord(Entity entity, string key);
        IDictionary<string, object> GetRecord(Entity entity, params object[] key);
        
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