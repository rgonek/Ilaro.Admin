using System;
using System.Data.Common;

namespace Ilaro.Admin.Core.DataAccess
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