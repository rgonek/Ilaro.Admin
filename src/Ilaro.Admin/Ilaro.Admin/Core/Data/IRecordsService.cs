using Ilaro.Admin.Models;
using System.Collections.Specialized;

namespace Ilaro.Admin.Core.Data
{
    public interface IRecordsService
    {
        PagedRecords GetRecords(
            Entity entity,
            NameValueCollection request,
            TableInfo tableInfo);

        PagedRecords GetChanges(
            Entity entityChangesFor,
            string key,
            NameValueCollection request,
            TableInfo tableInfo);
    }
}