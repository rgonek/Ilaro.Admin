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
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecords _source;

        private const string SqlFormat =
@"-- update record
UPDATE {0} SET 
    {1} 
    WHERE {2};
-- return record id
SELECT @{3};
-- update foreign entities records";

        /// <summary>
        /// UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
        /// </summary>
        private const string RelatedRecordsUpdateSqlFormat =
@"UPDATE {0} SET {1} = @{2} 
WHERE {3} In ({4});";

        public RecordsUpdater(
            IExecutingDbCommand executor,
            IFetchingRecords source)
        {
            if (executor == null)
                throw new ArgumentNullException("executor");
            if (source == null)
                throw new ArgumentNullException("source");

            _executor = executor;
            _source = source;
        }

        public bool Update(Entity entity)
        {
            try
            {
                var cmd = CreateCommand(entity);

                // TODO: get info about changed properties
                var result = _executor
                    .ExecuteWithChanges(cmd, new ChangeInfo(entity.Name, EntityChangeType.Update));

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

            var cmd = DB.CreateCommand();
            var counter = 0;
            foreach (var property in
                entity.CreateProperties(getForeignCollection: false).Where(x => x.IsKey == false))
            {
                AddParam(cmd, property);
                sbKeys.AppendFormat("\t{0} = @{1}, \r\n", property.ColumnName, counter++);
            }
            cmd.AddParams(entity.Key.Select(x => x.Value.Raw).ToArray());
            cmd.AddParam(entity.JoinedKeyValue);
            var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4);
            var whereParts = new List<string>();
            foreach (var key in entity.Key)
            {
                whereParts.Add("{0} = @{1}".Fill(key.ColumnName, counter++));
            }
            var wherePart = string.Join(" AND ", whereParts);
            cmd.CommandText = SqlFormat.Fill(entity.TableName, keys, wherePart, counter);

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
                    .Select(x => x.KeyValue.FirstOrDefault())
                    .Except(property.Value.Values.Select(x => x.ToStringSafe()))
                    .ToList();
                if (idsToRemoveRelation.Any())
                {
                    var removeValues = string.Join(",", idsToRemoveRelation.Select(x => "@" + paramIndex++));
                    sbUpdates.AppendLine();
                    sbUpdates.AppendFormat(
                        RelatedRecordsUpdateSqlFormat,
                        property.ForeignEntity.TableName,
                        entity.Key.FirstOrDefault().ColumnName,
                        paramIndex++,
                        property.ForeignEntity.Key.FirstOrDefault().ColumnName,
                        removeValues);
                    cmd.AddParams(idsToRemoveRelation.OfType<object>().ToArray());
                    cmd.AddParam(null);
                }

                var values = string.Join(",", property.Value.Values.Select(x => "@" + paramIndex++));
                sbUpdates.AppendLine();
                sbUpdates.AppendFormat(
                    RelatedRecordsUpdateSqlFormat,
                    property.ForeignEntity.TableName,
                    entity.Key.FirstOrDefault().ColumnName,
                    paramIndex++,
                    property.ForeignEntity.Key.FirstOrDefault().ColumnName,
                    values);
                cmd.AddParams(property.Value.Values.ToArray());
                cmd.AddParam(entity.Key.FirstOrDefault().Value.Raw);
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