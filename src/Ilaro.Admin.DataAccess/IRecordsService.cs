using Ilaro.Admin.Commons.Models;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Ilaro.Admin.Commons.Data
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

        IList<ChangeRow> GetLastChanges(int quantity);
    }
}