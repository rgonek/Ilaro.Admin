using System;
using System.Data.Common;

namespace Ilaro.Admin.Core.Data
{
    public interface IExecutingDbCommand
    {
        object ExecuteWithChanges(
            DbCommand cmd, 
            EntityRecord entityRecord, 
            EntityChangeType changeType, 
            Func<string> changeDescriber = null);
    }
}