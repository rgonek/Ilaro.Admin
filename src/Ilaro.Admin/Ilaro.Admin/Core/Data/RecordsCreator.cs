using System;
using System.Collections.Generic;
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
        private readonly IIlaroAdmin _admin;
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
WHERE {2};";

        public RecordsCreator(IIlaroAdmin admin, IExecutingDbCommand executor)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));

            _admin = admin;
            _executor = executor;
        }

        public string Create(Entity entity, Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entity);

                var result = _executor.ExecuteWithChanges(
                    cmd, 
                    entity.Name, 
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

            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            foreach (var property in entity
                .GetDefaultCreateProperties(getForeignCollection: false)
                .WhereIsNotSkipped())
            {
                sbKeys.AppendFormat("{0},", property.Column);
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
                var values =
                    property.Value.Values.Select(
                        x => x.ToStringSafe().Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                var whereParts = new List<string>();
                for (int i = 0; i < property.ForeignEntity.Key.Count; i++)
                {
                    var key = property.ForeignEntity.Key[i];
                    var joinedValues = string.Join(",", values.Select(x => "@" + paramIndex++));
                    whereParts.Add("{0} In ({1})".Fill(key.Column, joinedValues));
                    cmd.AddParams(values.Select(x => x[i]).OfType<object>().ToArray());
                }
                var wherePart = string.Join(" AND ", whereParts);
                sbUpdates.AppendLine();
                sbUpdates.AppendFormat(
                    RelatedRecordsUpdateSqlFormat,
                    property.ForeignEntity.TableName,
                    entity.Key.FirstOrDefault().Column,
                    wherePart);
            }

            cmd.CommandText += sbUpdates.ToString();
        }

        private static void AddParam(DbCommand cmd, Property property)
        {
            if (property.TypeInfo.IsFileStoredInDb)
                cmd.AddParam(property.Value.Raw, DbType.Binary);
            else
            {
                if (property.Value.Raw.IsBehavior(DefaultValueBehavior.Now) ||
                    property.Value.Raw.IsBehavior(DefaultValueBehavior.NowOnCreate))
                {
                    cmd.AddParam(DateTime.Now);
                }
                else if (property.Value.Raw.IsBehavior(DefaultValueBehavior.UtcNow) ||
                    property.Value.Raw.IsBehavior(DefaultValueBehavior.UtcNowOnCreate))
                {
                    cmd.AddParam(DateTime.UtcNow);
                }
                else
                {
                    cmd.AddParam(property.Value.Raw);
                }
            }
        }
    }
}