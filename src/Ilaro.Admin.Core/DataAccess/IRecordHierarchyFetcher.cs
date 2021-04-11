using Ilaro.Admin.Core.Models;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordHierarchyFetcher
    {
        RecordHierarchy GetRecordHierarchy(
            EntityRecord entityRecord,
            IList<PropertyDeleteOption> options = null);
        EntityHierarchy GetEntityHierarchy(
            Entity entity,
            IList<PropertyDeleteOption> options = null);
    }
}