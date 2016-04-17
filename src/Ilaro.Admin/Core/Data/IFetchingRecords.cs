using System.Collections.Generic;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core.Data
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
            int? take = null);
    }
}