using System.Data.Common;

namespace Ilaro.Admin.Core.Data
{
    public interface IExecutingDbCommand
    {
        object ExecuteWithChanges(DbCommand cmd, ChangeInfo changeInfo);
    }
}