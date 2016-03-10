using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsUpdater : IUpdatingRecords
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsUpdater));
        private readonly IIlaroAdmin _admin;
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecords _source;

        private const string SqlFormat =
@"-- update record
UPDATE {0} SET 
    {1} 
    WHERE {2};
";

        private const string SqlReturnRecordIdPart =
@"-- return record id
SELECT @{0};
-- update foreign entities records";

        /// <summary>
        /// UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
        /// </summary>
        private const string RelatedRecordsUpdateSqlFormat =
@"UPDATE {0} SET {1} = @{2} 
WHERE {3};";

        public RecordsUpdater(
            IIlaroAdmin admin,
            IExecutingDbCommand executor,
            IFetchingRecords source)
        {
            if (admin == null)
                throw new ArgumentNullException("admin");
            if (executor == null)
                throw new ArgumentNullException("executor");
            if (source == null)
                throw new ArgumentNullException("source");

            _admin = admin;
            _executor = executor;
            _source = source;
        }

        public bool Update(Entity entity, Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entity);

                // TODO: get info about changed properties
                var result = _executor
                    .ExecuteWithChanges(cmd, entity.Name, EntityChangeType.Update, changeDescriber);

                return result != null;
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

        protected virtual DbCommand CreateBaseCommand(Entity entity)
        {
            var sbKeys = new StringBuilder();

            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            var updateProperties = entity.CreateProperties(getForeignCollection: false)
                .Where(x => x.IsKey == false)
                .WhereIsNotSkipped().ToList();
            if (updateProperties.Any())
            {
                foreach (var property in updateProperties)
                {
                    AddParam(cmd, property);
                    sbKeys.AppendFormat("\t{0} = @{1}, \r\n", property.ColumnName, counter++);
                }
                cmd.AddParams(entity.Key.Select(x => x.Value.Raw).ToArray());
                var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4);
                var whereParts = new List<string>();
                foreach (var key in entity.Key)
                {
                    whereParts.Add("{0} = @{1}".Fill(key.ColumnName, counter++));
                }
                var wherePart = string.Join(" AND ", whereParts);
                cmd.CommandText = SqlFormat.Fill(entity.TableName, keys, wherePart);
            }
            cmd.AddParam(entity.JoinedKeyValue);
            cmd.CommandText += SqlReturnRecordIdPart.Fill(counter);

            return cmd;
        }

        private void AddForeignsUpdate(DbCommand cmd, Entity entity)
        {
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var property in entity.GetForeignsForUpdate())
            {
                var actualRecords = _source.GetRecords(
                    property.ForeignEntity,
                    new List<BaseFilter>
                    {
                        new ForeignEntityFilter(
                            entity.Key.FirstOrDefault(), 
                            entity.Key.FirstOrDefault().Value.Raw.ToStringSafe())
                    }).Records;
                var idsToRemoveRelation = actualRecords
                    .Select(x => x.JoinedKeyValue)
                    .Except(property.Value.Values.Select(x => x.ToStringSafe()))
                    .ToList();
                if (idsToRemoveRelation.Any())
                {
                    var values2 =
                        idsToRemoveRelation.Select(
                            x => x.Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                    var whereParts2 = new List<string>();
                    for (int i = 0; i < property.ForeignEntity.Key.Count; i++)
                    {
                        var key = property.ForeignEntity.Key[i];
                        var joinedValues = string.Join(",", values2.Select(x => "@" + paramIndex++));
                        whereParts2.Add("{0} In ({1})".Fill(key.ColumnName, joinedValues));
                        cmd.AddParams(values2.Select(x => x[i]).OfType<object>().ToArray());
                    }
                    var wherePart2 = string.Join(" AND ", whereParts2);
                    sbUpdates.AppendLine();
                    sbUpdates.AppendLine("-- set to null update");
                    sbUpdates.AppendFormat(
                        RelatedRecordsUpdateSqlFormat,
                        property.ForeignEntity.TableName,
                        entity.Key.FirstOrDefault().ColumnName,
                        paramIndex++,
                        wherePart2);
                    cmd.AddParam(null);
                }

                var values =
                    property.Value.Values.Select(
                        x => x.ToStringSafe().Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                var whereParts = new List<string>();
                for (int i = 0; i < property.ForeignEntity.Key.Count; i++)
                {
                    var key = property.ForeignEntity.Key[i];
                    var joinedValues = string.Join(",", values.Select(x => "@" + paramIndex++));
                    whereParts.Add("{0} In ({1})".Fill(key.ColumnName, joinedValues));
                    cmd.AddParams(values.Select(x => x[i]).OfType<object>().ToArray());
                }
                var wherePart = string.Join(" AND ", whereParts);
                sbUpdates.AppendLine();
                sbUpdates.AppendFormat(
                    RelatedRecordsUpdateSqlFormat,
                    property.ForeignEntity.TableName,
                    entity.Key.FirstOrDefault().ColumnName,
                    paramIndex++,
                    wherePart);
                cmd.AddParam(entity.Key.FirstOrDefault().Value.Raw);
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
                    property.Value.Raw.IsBehavior(DefaultValueBehavior.NowOnUpdate))
                {
                    cmd.AddParam(DateTime.Now);
                }
                else if (property.Value.Raw.IsBehavior(DefaultValueBehavior.UtcNow) ||
                    property.Value.Raw.IsBehavior(DefaultValueBehavior.UtcNowOnUpdate))
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