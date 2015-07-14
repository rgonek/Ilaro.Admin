using System;
using System.Data.Common;
using Ilaro.Admin.Extensions;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class DbCommandExecutor : IExecutingDbCommand
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(DbCommandExecutor));
        private readonly IProvidingUser _user;

        public DbCommandExecutor(IProvidingUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _user = user;
        }

        public object ExecuteWithChanges(DbCommand cmd, ChangeInfo changeInfo)
        {
            _log.DebugFormat("Executing command: \r\n {0}", cmd.CommandText);
            object result;
            using (var conn = DB.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.Transaction = tx;
                    result = cmd.ExecuteScalar();

                    if (Admin.IsChangesEnabled)
                    {
                        var changeCmd = CreateChangeCommand(changeInfo, result.ToString());
                        _log.DebugFormat("Executing change command: \r\n {0}", changeCmd.CommandText);
                        changeCmd.Connection = conn;
                        changeCmd.Transaction = tx;
                        changeCmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    tx.Rollback();
                    throw;
                }
            }

            return result;
        }

        private DbCommand CreateChangeCommand(ChangeInfo changeInfo, string keyValue)
        {
            var cmd = DB.CreateCommand();

            var sql =
@"INSERT INTO {0} ([EntityName], [EntityKey], [ChangeType], [Description], [ChangedOn], [ChangedBy])
VALUES (@0,@1,@2,@3,@4,@5);".Fill(Admin.ChangeEntity.TableName);

            cmd.AddParam(changeInfo.EntityName);
            cmd.AddParam(keyValue);
            cmd.AddParam(changeInfo.Type);
            cmd.AddParam(changeInfo.Description);
            cmd.AddParam(DateTime.UtcNow);
            cmd.AddParam(_user.Current());

            cmd.CommandText = sql;

            return cmd;
        }
    }
}