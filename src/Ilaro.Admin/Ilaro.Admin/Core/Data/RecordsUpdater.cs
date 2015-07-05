using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Extensions2;
using Ilaro.Admin.Filters;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsUpdater : IUpdatingRecords
    {
        private readonly Notificator _notificator;
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecords _source;

        private const string SqlFormat =
@"UPDATE {0} SET {1} WHERE {2} = @{3};
SELECT @{3}";

        /// <summary>
        /// UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
        /// </summary>
        private const string RelatedRecordsUpdateSqlFormat =
            "UPDATE {0} SET {1} = @{2} WHERE {3} In ({4});";

        public RecordsUpdater(
            Notificator notificator,
            IExecutingDbCommand executor,
            IFetchingRecords source)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (executor == null)
                throw new ArgumentNullException("executor");
            if (source == null)
                throw new ArgumentNullException("source");

            _notificator = notificator;
            _executor = executor;
            _source = source;
        }

        public bool Update(Entity entity)
        {
            var cmd = CreateCommand(entity);

            // TODO: get info about changed properties
            var result = (int)_executor
                .ExecuteWithChanges(cmd, new ChangeInfo(entity.Name, EntityChangeType.Update));

            entity.ClearPropertiesValues();

            return result > 0;
        }

        private DbCommand CreateCommand(Entity entity)
        {
            var cmd = CreateBaseCommand(entity);
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
                sbKeys.AppendFormat("{0} = @{1}, \r\n", property.ColumnName, counter);
                counter++;
            }
            cmd.AddParam(entity.Key.Value.Raw);
            var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4);
            cmd.CommandText = SqlFormat.Fill(entity.TableName, keys, entity.Key.ColumnName, counter);

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
                    new List<IEntityFilter>
                    {
                        new ForeignEntityFilter(
                            entity.Key, 
                            entity.Key.Value.Raw.ToStringSafe())
                    });
                var idsToRemoveRelation = actualRecords
                    .Select(x => x.KeyValue)
                    .Except(property.Value.Values.OfType<string>())
                    .ToList();
                if (idsToRemoveRelation.Any())
                {
                    var removeValues = string.Join(",", idsToRemoveRelation.Select(x => "@" + paramIndex++));
                    sbUpdates.AppendLine();
                    sbUpdates.AppendFormat(
                        RelatedRecordsUpdateSqlFormat,
                        property.ForeignEntity.TableName,
                        entity.Key.ColumnName,
                        paramIndex++,
                        property.ForeignEntity.Key.ColumnName,
                        removeValues);
                    cmd.AddParams(idsToRemoveRelation);
                    cmd.AddParams(null);
                }

                var values = string.Join(",", property.Value.Values.Select(x => "@" + paramIndex++));
                cmd.CommandText +=
                    Environment.NewLine +
                    RelatedRecordsUpdateSqlFormat.Fill(
                        property.ForeignEntity.TableName,
                        entity.Key.ColumnName,
                        paramIndex++,
                        property.ForeignEntity.Key.ColumnName,
                        values);
                cmd.AddParams(property.Value.Values);
                cmd.AddParams(entity.Key.Value.Raw);
            }
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