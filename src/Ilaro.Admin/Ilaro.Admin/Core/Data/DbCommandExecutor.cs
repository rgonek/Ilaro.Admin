using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Text;
using System.Web;
using Ilaro.Admin.Extensions;
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
            var cmd = DB.CreateCommand();

            var sql =
@"INSERT INTO {0} ([EntityName], [EntityKey], [ChangeType], [Description], [ChangedOn], [ChangedBy])
VALUES (@0,@1,@2,@3,@4,@5);".Fill(AdminInitialise.ChangeEntity.TableName);

            cmd.AddParam(changeInfo.EntityName);
            cmd.AddParam(keyValue);
            cmd.AddParam(changeInfo.Type);
            cmd.AddParam(changeInfo.Description);
            cmd.AddParam(DateTime.UtcNow);
            cmd.AddParam(HttpContext.Current.User.Identity.Name);

            cmd.CommandText = sql;

            return cmd;
        }
    }
}