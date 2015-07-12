using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsCreator : ICreatingRecords
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsCreator));
        private readonly IExecutingDbCommand _executor;

        private const string SqlFormat =
@"-- insert record
INSERT INTO {0} ({1}) 
VALUES ({2});
-- return record id
DECLARE @newID {3} = {4};
SELECT @newID;
-- update foreign entities records";

        /// <summary>
        /// UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
        /// </summary>
        private const string RelatedRecordsUpdateSqlFormat =
@"UPDATE {0} SET {1} = @newID 
WHERE {2} In ({3});";

        public RecordsCreator(IExecutingDbCommand executor)
        {
            if (executor == null)
                throw new ArgumentNullException("executor");

            _executor = executor;
        }

        public string Create(Entity entity)
        {
            try
            {
                var cmd = CreateCommand(entity);

                var result = _executor
                    .ExecuteWithChanges(cmd, new ChangeInfo(entity.Name, EntityChangeType.Insert));

                return result.ToStringSafe();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw ex;
            }
        }

        private DbCommand CreateCommand(Entity entity)
        {
            var cmd = CreateBaseCommand(entity);
            if (entity.Key.Count == 1)
                AddForeignsUpdate(cmd, entity);

            return cmd;
        }

        private DbCommand CreateBaseCommand(Entity entity)
        {
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();

            var cmd = DB.CreateCommand();
            var counter = 0;
            foreach (var property in entity.CreateProperties(getForeignCollection: false))
            {
                sbKeys.AppendFormat("{0},", property.ColumnName);
                sbVals.AppendFormat("@{0},", counter);
                AddParam(cmd, property);
                counter++;
            }
            var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
            var vals = sbVals.ToString().Substring(0, sbVals.Length - 1);
            var idType = "int";
            var insertedId = "SCOPE_IDENTITY()";
            if (entity.Key.Count > 1 || entity.Key.FirstOrDefault().TypeInfo.IsString)
            {
                idType = "nvarchar(max)";
                insertedId = "@" + counter;
                cmd.AddParam(entity.JoinedKeyValue);
            }
            var sql = SqlFormat.Fill(entity.TableName, keys, vals, idType, insertedId);
            cmd.CommandText = sql;

            return cmd;
        }

        private void AddForeignsUpdate(DbCommand cmd, Entity entity)
        {
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var property in
                entity.GetForeignsForUpdate().Where(x => x.Value.Values.IsNullOrEmpty<object>() == false))
            {
                var values = string.Join(",", property.Value.Values.Select(x => "@" + paramIndex++));
                sbUpdates.AppendLine();
                sbUpdates.AppendFormat(
                    RelatedRecordsUpdateSqlFormat,
                    property.ForeignEntity.TableName,
                    entity.Key.FirstOrDefault().ColumnName,
                    property.ForeignEntity.Key.FirstOrDefault().ColumnName,
                    values);
                cmd.AddParams(property.Value.Values.ToArray());
            }

            cmd.CommandText += sbUpdates.ToString();
        }

        private static void AddParam(DbCommand cmd, Property property)
        {
            if (property.TypeInfo.IsFileStoredInDb)
                cmd.AddParam(property.Value.Raw, DbType.Binary);
            else
                cmd.AddParam(property.Value.Raw);
        }
    }
}