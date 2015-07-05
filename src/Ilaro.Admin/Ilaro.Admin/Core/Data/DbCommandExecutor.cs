using System;
using System.Data.Common;
using System.Dynamic;
using System.Web;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class DbCommandExecutor : IExecutingDbCommand
    {
        private readonly Notificator _notificator;

        public DbCommandExecutor(Notificator notificator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");

            _notificator = notificator;
        }

        public object ExecuteWithChanges(DbCommand cmd, ChangeInfo changeInfo)
        {
            object result;
            using (var conn = DB.OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.Transaction = tx;
                    result = cmd.ExecuteScalar();

                    if (AdminInitialise.IsChangesEnabled)
                    {
                        var changeCmd = CreateChangeCommand(changeInfo, result.ToString());
                        changeCmd.Connection = conn;
                        changeCmd.Transaction = tx;
                        changeCmd.ExecuteNonQuery();
                    }

                    throw new Exception("test");

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    result = null;
                    _notificator.Error(ex.Message);
                    tx.Rollback();
                }
            }

            return result;
        }

        private DbCommand CreateChangeCommand(ChangeInfo changeInfo, string keyValue)
        {
            var changes = new DynamicModel(
                AdminInitialise.ConnectionString,
                AdminInitialise.ChangeEntity.TableName,
                "EntityChangeId");

            dynamic changeRecord = new ExpandoObject();
            changeRecord.EntityName = changeInfo.EntityName;
            changeRecord.EntityKey = keyValue;
            changeRecord.ChangeType = changeInfo.Type;
            changeRecord.Description = changeInfo.Description;
            changeRecord.ChangedOn = DateTime.UtcNow;
            changeRecord.ChangedBy = HttpContext.Current.User.Identity.Name;

            DbCommand cmd = changes.CreateInsertCommand(changeRecord);

            return cmd;
        }
    }
}