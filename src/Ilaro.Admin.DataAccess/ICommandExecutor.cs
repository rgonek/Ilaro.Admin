using Ilaro.Admin.Commons;
using Ilaro.Admin.DataAccess.Audit;
using System;
using System.Data.Common;

namespace Ilaro.Admin.DataAccess
{
    public interface ICommandExecutor
    {
        object ExecuteWithChanges(
            DbCommand cmd,
            EntityRecord entityRecord,
            EntityChangeType changeType,
            Func<string> changeDescriber = null);
    }
}