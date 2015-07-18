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

        public object ExecuteWithChanges(
            DbCommand cmd,
            string entityName,
            EntityChangeType changeType,
            Func<string> changeDescriber = null)
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

                    if (result != null)
                    {
                        if (Admin.IsChangesEnabled)
                        {
                            var changeCmd = CreateChangeCommand(entityName, changeType, result.ToString(), changeDescriber);
                            _log.DebugFormat("Executing change command: \r\n {0}", changeCmd.CommandText);
                            changeCmd.Connection = conn;
                            changeCmd.Transaction = tx;
                            changeCmd.ExecuteNonQuery();
                        }

                        _log.Debug("Commit transaction");
                        tx.Commit();
                    }
                    else
                    {
                        Rollback(tx);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    Rollback(tx);
                    throw;
                }
            }

            return result;
        }

        private void Rollback(DbTransaction tx)
        {
            try
            {
                tx.Rollback();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        private DbCommand CreateChangeCommand(
            string entityName,
            EntityChangeType changeType,
            string keyValue,
            Func<string> changeDescriber = null)
        {
            var cmd = DB.CreateCommand();

            var sql =
@"INSERT INTO {0} ([EntityName], [EntityKey], [ChangeType], [Description], [ChangedOn], [ChangedBy])
VALUES (@0,@1,@2,@3,@4,@5);".Fill(Admin.ChangeEntity.TableName);

            cmd.AddParam(entityName);
            cmd.AddParam(keyValue);
            cmd.AddParam(changeType);
            cmd.AddParam(changeDescriber == null ? null : changeDescriber());
            cmd.AddParam(DateTime.UtcNow);
            cmd.AddParam(_user.Current());

            cmd.CommandText = sql;

            return cmd;
        }
    }
}