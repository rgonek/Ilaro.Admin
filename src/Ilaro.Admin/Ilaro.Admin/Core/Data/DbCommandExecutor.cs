using System;
using System.Data.Common;
using System.Dynamic;
using System.Web;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class DbCommandExecutor : IExecutingDbCommand
    {
        public object ExecuteWithChanges(DbCommand cmd, ChangeInfo changeInfo)
        {
            object result;
            var db = new DynamicModel(AdminInitialise.ConnectionString);
            using (var conn = db.OpenConnection())
            {
                using (var tx = conn.BeginTransaction())
                {
                    cmd.Connection = conn;
                    cmd.Transaction = tx;
                    result = cmd.ExecuteScalar();

                    if (AdminInitialise.IsChangesEnabled)
                    {
                        var changeCmd = CreateChangeCommand(changeInfo, result.ToString());

                        changeCmd.Parameters[1].Value = result;
                        changeCmd.Connection = conn;
                        changeCmd.Transaction = tx;
                        changeCmd.ExecuteNonQuery();
                    }

                    tx.Commit();
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