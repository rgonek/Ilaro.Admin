using Ilaro.Admin.Commons.Models;
using System.Collections.Generic;

namespace Ilaro.Admin.Commons.Data
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