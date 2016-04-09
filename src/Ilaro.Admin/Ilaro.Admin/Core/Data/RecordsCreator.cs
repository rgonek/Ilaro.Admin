using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Massive;
using Ilaro.Admin.Core.Extensions;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsCreator : ICreatingRecords
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsCreator));
        private readonly IIlaroAdmin _admin;
        private readonly IExecutingDbCommand _executor;
        private readonly IProvidingUser _user;

        public RecordsCreator(
            IIlaroAdmin admin,
            IExecutingDbCommand executor,
            IProvidingUser user)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _admin = admin;
            _executor = executor;
            _user = user;
        }

        public string Create(EntityRecord entityRecord, Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord);

                var result = _executor.ExecuteWithChanges(
                    cmd,
                    entityRecord,
                    EntityChangeType.Insert,
                    changeDescriber);

                return result.ToStringSafe();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        private DbCommand CreateCommand(EntityRecord entityRecord)
        {
            var cmd = CreateBaseCommand(entityRecord);
            if (entityRecord.Key.Count() == 1)
                AddForeignsUpdate(cmd, entityRecord);

            return cmd;
        }

        private DbCommand CreateBaseCommand(EntityRecord entityRecord)
        {
            var sbColumns = new StringBuilder();
            var sbValues = new StringBuilder();

            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            foreach (var propertyValue in entityRecord.Values
                .WhereIsNotSkipped()
                .WhereIsNotOneToMany()
                .Where(x => x.Property.IsAutoKey == false))
            {
                sbColumns.AppendFormat("{0},", propertyValue.Property.Column);
                sbValues.AppendFormat("@{0},", counter);
                AddParam(cmd, propertyValue);
                counter++;
            }
            var columns = sbColumns.ToString().Substring(0, sbColumns.Length - 1);
            var values = sbValues.ToString().Substring(0, sbValues.Length - 1);
            var idType = "int";
            var insertedId = "SCOPE_IDENTITY()";
            if (entityRecord.Key.Count > 1 || entityRecord.Key.FirstOrDefault().Property.TypeInfo.IsString)
            {
                idType = "nvarchar(max)";
                insertedId = "@" + counter;
                cmd.AddParam(entityRecord.JoinedKeyValue);
            }
            var table = entityRecord.Entity.Table;

            cmd.CommandText =
$@"-- insert record
INSERT INTO {table} ({columns}) 
VALUES ({values});
-- return record id
DECLARE @newID {idType} = {insertedId};
SELECT @newID;
-- update foreign entities records";

            return cmd;
        }

        private void AddForeignsUpdate(DbCommand cmd, EntityRecord entityRecord)
        {
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var propertyValue in entityRecord.Values
                .WhereOneToMany()
                .Where(value => value.Values.IsNullOrEmpty() == false))
            {
                var values =
                    propertyValue.Values.Select(
                        x => x.ToStringSafe().Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                var whereParts = new List<string>();
                for (int i = 0; i < propertyValue.Property.ForeignEntity.Key.Count; i++)
                {
                    var key = propertyValue.Property.ForeignEntity.Key[i];
                    var joinedValues = string.Join(",", values.Select(x => "@" + paramIndex++));
                    whereParts.Add("{0} In ({1})".Fill(key.Column, joinedValues));
                    cmd.AddParams(values.Select(x => x[i]).OfType<object>().ToArray());
                }
                var constraintSeparator = Environment.NewLine + "   AND ";
                var constraints = string.Join(constraintSeparator, whereParts);
                sbUpdates.AppendLine();

                var table = propertyValue.Property.ForeignEntity.Table;
                var foreignKey = entityRecord.Entity.Key.FirstOrDefault().Column;

                sbUpdates.Append($@"UPDATE {table}
   SET {foreignKey} = @newID 
 WHERE {constraints};");
            }

            cmd.CommandText += sbUpdates.ToString();
        }

        private void AddParam(DbCommand cmd, PropertyValue propertyValue)
        {
            if (propertyValue.Raw is ValueBehavior)
            {
                switch (propertyValue.Raw as ValueBehavior?)
                {
                    case ValueBehavior.Now:
                        cmd.AddParam(DateTime.Now);
                        break;
                    case ValueBehavior.UtcNow:
                        cmd.AddParam(DateTime.UtcNow);
                        break;
                    case ValueBehavior.CurrentUserId:
                        cmd.AddParam((int)_user.CurrentId());
                        break;
                    case ValueBehavior.CurrentUserName:
                        cmd.AddParam(_user.CurrentUserName());
                        break;
                }
            }
            else
            {
                if (propertyValue.Property.TypeInfo.IsFileStoredInDb)
                    cmd.AddParam(propertyValue.Raw, DbType.Binary);
                else
                    cmd.AddParam(propertyValue.Raw);
            }
        }
    }
}