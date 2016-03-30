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

        public string Create(EntityRecord entityRecord, Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord);

                var result = _executor.ExecuteWithChanges(
                    cmd,
                    entityRecord.Entity.Name,
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
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();

            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            foreach (var propertyValue in entityRecord.Values
                .WhereIsNotSkipped()
                .WhereIsNotOneToMany()
                .Where(x => x.Property.IsAutoKey == false))
            {
                sbKeys.AppendFormat("{0},", propertyValue.Property.Column);
                sbVals.AppendFormat("@{0},", counter);
                AddParam(cmd, propertyValue);
                counter++;
            }
            var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
            var vals = sbVals.ToString().Substring(0, sbVals.Length - 1);
            var idType = "int";
            var insertedId = "SCOPE_IDENTITY()";
            if (entityRecord.Key.Count > 1 || entityRecord.Key.FirstOrDefault().Property.TypeInfo.IsString)
            {
                idType = "nvarchar(max)";
                insertedId = "@" + counter;
                cmd.AddParam(entityRecord.JoinedKeyValue);
            }
            var sql = SqlFormat.Fill(entityRecord.Entity.TableName, keys, vals, idType, insertedId);
            cmd.CommandText = sql;

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
                var wherePart = string.Join(" AND ", whereParts);
                sbUpdates.AppendLine();
                sbUpdates.AppendFormat(
                    RelatedRecordsUpdateSqlFormat,
                    propertyValue.Property.ForeignEntity.TableName,
                    entityRecord.Entity.Key.FirstOrDefault().Column,
                    wherePart);
            }

            cmd.CommandText += sbUpdates.ToString();
        }

        private static void AddParam(
            DbCommand cmd,
            PropertyValue propertyValue)
        {
            if (propertyValue.Property.TypeInfo.IsFileStoredInDb)
                cmd.AddParam(propertyValue.Raw, DbType.Binary);
            else
            {
                if (propertyValue.Raw.IsBehavior(DefaultValueBehavior.Now) ||
                    propertyValue.Raw.IsBehavior(DefaultValueBehavior.NowOnCreate))
                {
                    cmd.AddParam(DateTime.Now);
                }
                else if (propertyValue.Raw.IsBehavior(DefaultValueBehavior.UtcNow) ||
                    propertyValue.Raw.IsBehavior(DefaultValueBehavior.UtcNowOnCreate))
                {
                    cmd.AddParam(DateTime.UtcNow);
                }
                else
                {
                    cmd.AddParam(propertyValue.Raw);
                }
            }
        }
    }
}