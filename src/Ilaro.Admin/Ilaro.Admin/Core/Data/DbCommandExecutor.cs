using System;
using System.Data.Common;
using Ilaro.Admin.Extensions;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class DbCommandExecutor : IExecutingDbCommand
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(DbCommandExecutor));
        private readonly IIlaroAdmin _admin;
        private readonly IProvidingUser _user;

        public DbCommandExecutor(IIlaroAdmin admin, IProvidingUser user)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _admin = admin;
            _user = user;
        }

        public object ExecuteWithChanges(
            DbCommand cmd,
            EntityRecord entityRecord,
            EntityChangeType changeType,
            Func<string> changeDescriber = null)
        {
            _log.DebugFormat("Executing command: \r\n {0}", cmd.CommandText);
            object result;
            using (var conn = DB.OpenConnection(_admin.ConnectionStringName))
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.Transaction = tx;
                    result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        if (_admin.IsChangesEnabled)
                        {
                            var changeCmd = CreateChangeCommand(entityRecord, changeType, result.ToString(), changeDescriber);
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
            EntityRecord entityRecord,
            EntityChangeType changeType,
            string keyValue,
            Func<string> changeDescriber = null)
        {
            if(changeType == EntityChangeType.Insert)
            {
                entityRecord.SetKeyValue(keyValue);
            }

            var cmd = DB.CreateCommand(_admin.ConnectionStringName);

            var changeEntity = _admin.ChangeEntity;
            var table = changeEntity.Table;
            var entityNameColumn = changeEntity["EntityName"].Column;
            var entityKeyColumn = changeEntity["EntityKey"].Column;
            var changeTypeColumn = changeEntity["ChangeType"].Column;
            var recordDisplayColumn = changeEntity["RecordDisplayName"].Column;
            var descriptionColumn = changeEntity["Description"].Column;
            var changedOnColumn = changeEntity["ChangedOn"].Column;
            var changedByColumn = changeEntity["ChangedBy"].Column;

            var sql =
$@"INSERT INTO {table} ({entityNameColumn}, {entityKeyColumn}, {changeTypeColumn}, {recordDisplayColumn}, {descriptionColumn}, {changedOnColumn}, {changedByColumn})
VALUES (@0,@1,@2,@3,@4,@5,@6);";

            cmd.AddParam(entityRecord.Entity.Name);
            cmd.AddParam(keyValue);
            cmd.AddParam(changeType);
            cmd.AddParam(entityRecord.ToString());
            cmd.AddParam(changeDescriber == null ? null : changeDescriber());
            cmd.AddParam(DateTime.UtcNow);
            cmd.AddParam(_user.CurrentUserName());

            cmd.CommandText = sql;

            return cmd;
        }
    }
}