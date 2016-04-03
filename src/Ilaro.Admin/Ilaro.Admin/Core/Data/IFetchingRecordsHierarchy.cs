using Ilaro.Admin.Models;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IFetchingRecordsHierarchy
    {
        RecordHierarchy GetRecordHierarchy(
            EntityRecord entityRecord,
            IList<PropertyDeleteOption> options = null);
        EntityHierarchy GetEntityHierarchy(
            Entity entity,
            IList<PropertyDeleteOption> options = null);
    }
}