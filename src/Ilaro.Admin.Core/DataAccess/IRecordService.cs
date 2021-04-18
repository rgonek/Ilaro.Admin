using Ilaro.Admin.Core.Models;
using System.Collections.Specialized;

namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordService
    {
        PagedRecords GetRecords(
            Entity entity,
            NameValueCollection request,
            TableInfo tableInfo);
    }
}