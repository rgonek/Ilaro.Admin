using System;
using System.Data.Common;
using Dawn;
using Ilaro.Admin.Common;
using Ilaro.Admin.Commons;
using Ilaro.Admin.DataAccess.Audit;

namespace Ilaro.Admin.DataAccess
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly Func<ConnectionStringName> _connectionStringName;
        private readonly IUser _user;

        public CommandExecutor(Func<ConnectionStringName> connectionStringName, IUser user)
        {
            Guard.Argument(connectionStringName, nameof(connectionStringName)).NotNull();
            Guard.Argument(user, nameof(user)).NotNull();

            _connectionStringName = connectionStringName;
            _user = user;
        }

        public object ExecuteWithChanges(
            DbCommand cmd,
            EntityRecord entityRecord,
            EntityChangeType changeType,
            Func<string> changeDescriber = null)
        {
            //_log.DebugFormat("Executing command: \r\n {0}", cmd.CommandText);
            object result;
            using (var conn = DB.OpenConnection(_connectionStringName()))
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.Transaction = tx;
                    result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        //if (_admin.IsChangesEnabled)
                        //{
                        //    var changeCmd = CreateChangeCommand(entityRecord, changeType, result.ToString(), changeDescriber);
                        //    _log.DebugFormat("Executing change command: \r\n {0}", changeCmd.CommandText);
                        //    changeCmd.Connection = conn;
                        //    changeCmd.Transaction = tx;
                        //    changeCmd.ExecuteNonQuery();
                        //}

                        //_log.Debug("Commit transaction");
                        tx.Commit();
                    }
                    else
                    {
                        Rollback(tx);
                    }
                }
                catch (Exception ex)
                {
                    //_log.Error(ex);
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
                //_log.Error(ex);
            }
        }

//        private DbCommand CreateChangeCommand(
//            EntityRecord entityRecord,
//            EntityChangeType changeType,
//            string keyValue,
//            Func<string> changeDescriber = null)
//        {
//            if (changeType == EntityChangeType.Insert)
//            {
//                entityRecord.SetKeyValue(keyValue);
//            }

//            var cmd = DB.CreateCommand(_connectionStringName());

//            var changeEntity = _admin.ChangeEntity;
//            var table = changeEntity.Table;
//            var entityNameColumn = changeEntity["EntityName"].Column;
//            var entityKeyColumn = changeEntity["EntityKey"].Column;
//            var changeTypeColumn = changeEntity["ChangeType"].Column;
//            var recordDisplayColumn = changeEntity["RecordDisplayName"].Column;
//            var descriptionColumn = changeEntity["Description"].Column;
//            var changedOnColumn = changeEntity["ChangedOn"].Column;
//            var changedByColumn = changeEntity["ChangedBy"].Column;

//            var sql =
//$@"INSERT INTO {table} ({entityNameColumn}, {entityKeyColumn}, {changeTypeColumn}, {recordDisplayColumn}, {descriptionColumn}, {changedOnColumn}, {changedByColumn})
//VALUES (@0,@1,@2,@3,@4,@5,@6);";

//            cmd.AddParam(entityRecord.Entity.Name);
//            cmd.AddParam(keyValue);
//            cmd.AddParam(changeType);
//            cmd.AddParam(entityRecord.ToString());
//            cmd.AddParam(changeDescriber == null ? null : changeDescriber());
//            cmd.AddParam(DateTime.UtcNow);
//            cmd.AddParam(_user.UserName());

//            cmd.CommandText = sql;

//            return cmd;
//        }
    }
}