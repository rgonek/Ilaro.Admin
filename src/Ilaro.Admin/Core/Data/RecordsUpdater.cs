using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Massive;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Data.Extensions;
using Resources;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsUpdater : IUpdatingRecords
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsUpdater));
        private readonly IIlaroAdmin _admin;
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecords _source;
        private readonly IProvidingUser _user;

        public RecordsUpdater(
            IIlaroAdmin admin,
            IExecutingDbCommand executor,
            IFetchingRecords source,
            IProvidingUser user)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _admin = admin;
            _executor = executor;
            _source = source;
            _user = user;
        }

        public bool Update(
            EntityRecord entityRecord,
            object concurrencyCheckValue = null,
            Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord, concurrencyCheckValue);

                var previewSql = cmd.PreviewCommandText();
                _log.Debug(previewSql);

                var result = _executor.ExecuteWithChanges(
                    cmd,
                    entityRecord,
                    EntityChangeType.Update,
                    changeDescriber);

                if (result == null)
                    return false;
                if (result.Is(Const.ConcurrencyCheckError_ReturnValue))
                    throw new ConcurrencyCheckException(IlaroAdminResources.ConcurrencyCheckError);

                return true;
            }
            catch (ConcurrencyCheckException ex)
            {
                _log.Warn(ex);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        private DbCommand CreateCommand(
            EntityRecord entityRecord,
            object concurrencyCheckValue = null)
        {
            var cmd = CreateBaseCommand(entityRecord);
            AddConcurrencyCheck(cmd, entityRecord, concurrencyCheckValue);
            AddForeignsUpdate(cmd, entityRecord);
            AddManyToManyForeignsUpdate(cmd, entityRecord);

            return cmd;
        }

        protected virtual DbCommand CreateBaseCommand(EntityRecord entityRecord)
        {
            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            var updateProperties = entityRecord.Values
                .WhereIsNotSkipped()
                .WhereIsNotOneToMany()
                .Where(value => value.Property.IsCreatable)
                .Where(value => value.Property.IsKey == false)
                .DistinctBy(x => x.Property.Column)
                .ToList();
            if (updateProperties.Any())
            {
                var setsList = new List<string>();

                foreach (var propertyValue in updateProperties)
                {
                    AddParam(cmd, propertyValue);
                    var column = propertyValue.Property.Column;
                    var parameterName = (counter++).ToString();
                    setsList.Add($"{column} = @{parameterName}");
                }
                var setsSeparator = "," + Environment.NewLine + "       ";
                var sets = string.Join(setsSeparator, setsList);
                var table = entityRecord.Entity.Table;
                var constraints = GetConstraints(entityRecord.Keys, cmd);
                cmd.CommandText = $@"-- update record
UPDATE {table}
   SET {sets} 
 WHERE {constraints};
";
            }
            cmd.AddParam(entityRecord.JoinedKeysValues);
            var joinedKeyValueParameterName = counter.ToString();
            cmd.CommandText += $@"-- return record id
SELECT @{joinedKeyValueParameterName};";

            return cmd;
        }

        protected virtual string GetConstraints(
            IEnumerable<PropertyValue> keys,
            DbCommand cmd,
            string alias = null)
        {
            if (alias.HasValue())
                alias += ".";
            var counter = cmd.Parameters.Count;
            var whereParts = new List<string>();
            foreach (var key in keys)
            {
                var column = key.Property.Column;
                var parameterName = (counter++).ToString();
                whereParts.Add($"{alias}{key.Property.Column} = @{parameterName}");
            }
            var constraintSeparator = Environment.NewLine + "   AND ";
            var constraints = string.Join(constraintSeparator, whereParts);
            cmd.AddParams(keys.Select(value => value.Raw).ToArray());

            return constraints;
        }

        protected virtual void AddConcurrencyCheck(
            DbCommand cmd,
            EntityRecord entityRecord,
            object concurrencyCheckValue)
        {
            if (entityRecord.Entity.ConcurrencyCheckEnabled == false)
                return;

            if (concurrencyCheckValue == null)
                throw new InvalidOperationException(IlaroAdminResources.EmptyConcurrencyCheckValue);

            var property = entityRecord.Entity.Properties.FirstOrDefault(x => x.IsConcurrencyCheck);
            var concurrencyCheckParam = cmd.Parameters.Count.ToString();
            cmd.AddParam(concurrencyCheckValue);

            var concurrencyCheckConstraint =
                GetConcurrencyCheckValueSql(cmd, entityRecord, property, concurrencyCheckParam);

            var concurrencyCheckSql =
                $@"-- concurrency check
IF({concurrencyCheckConstraint})
BEGIN
    SELECT {Const
                    .ConcurrencyCheckError_ReturnValue};
    RETURN;
END
";

            cmd.CommandText = concurrencyCheckSql + cmd.CommandText;
        }

        private string GetConcurrencyCheckValueSql(
            DbCommand cmd,
            EntityRecord entityRecord,
            Property property,
            string concurrencyCheckParam)
        {
            string sql;
            if (property == null)
            {
                var entityChange = _admin.ChangeEntity;
                var changedOn = entityChange[nameof(IEntityChange.ChangedOn)];
                var changedEntityName = entityChange[nameof(IEntityChange.EntityName)];
                var changedEntityKey = entityChange[nameof(IEntityChange.EntityKey)];

                var changedEntityNameParam = cmd.Parameters.Count;
                cmd.AddParam(entityRecord.Entity.Name);
                var changedEntityKeyParam = cmd.Parameters.Count;
                cmd.AddParam(entityRecord.JoinedKeysValues);

                var constraints = GetConstraints(entityRecord.Keys, cmd, "[t0]");

                sql =
                    $@"@{concurrencyCheckParam} <=
    (SELECT TOP 1 [ec].{changedOn.Column}
      FROM {entityChange
                        .Table} as [ec]
     INNER JOIN {entityRecord.Entity.Table} as [t0] ON (
           [ec].{changedEntityName
                            .Column} = @{changedEntityNameParam}
       AND [ec].{changedEntityKey.Column} = @{changedEntityKeyParam}
       )
     WHERE {constraints}
     ORDER BY [ec].{changedOn
                                .Column} DESC)";
            }
            else
            {
                var constraints = GetConstraints(entityRecord.Keys, cmd);

                sql =
                    $@"@{concurrencyCheckParam} <>
    (SELECT {property.Column}
      FROM {entityRecord.Entity.Table}
     WHERE {constraints})";
            }

            return sql;
        }

        private void AddForeignsUpdate(DbCommand cmd, EntityRecord entityRecord)
        {
            if (entityRecord.Keys.Count > 1)
                return;
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var propertyValue in entityRecord.Values.WhereOneToMany()
                .Where(x => x.Property.IsManyToMany == false))
            {
                var actualRecords = _source.GetRecords(
                    propertyValue.Property.ForeignEntity,
                    new List<BaseFilter>
                    {
                        new ForeignEntityFilter(
                            entityRecord.Entity.Keys.FirstOrDefault(),
                            entityRecord.Keys.FirstOrDefault().Raw.ToStringSafe())
                    }).Records;
                var idsToRemoveRelation = actualRecords
                    .Select(x => x.JoinedKeysValues)
                    .Except(propertyValue.Values.Select(x => x.ToStringSafe()))
                    .ToList();
                if (idsToRemoveRelation.Any())
                {
                    var values2 =
                        idsToRemoveRelation.Select(
                            x => x.Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                    var whereParts2 = new List<string>();
                    var addNullSqlPart = true;
                    for (int i = 0; i < propertyValue.Property.ForeignEntity.Keys.Count; i++)
                    {
                        var key = propertyValue.Property.ForeignEntity.Keys[i];
                        var joinedValues = string.Join(",", values2.Select(x => "@" + paramIndex++));
                        whereParts2.Add("{0} In ({1})".Fill(key.Column, joinedValues));
                        addNullSqlPart = joinedValues.HasValue();
                        if (addNullSqlPart == false)
                            break;
                        cmd.AddParams(values2.Select(x => x[i]).OfType<object>().ToArray());
                    }
                    if (addNullSqlPart)
                    {
                        var wherePart2 = string.Join(" AND ", whereParts2);
                        sbUpdates.AppendLine();
                        sbUpdates.AppendLine("-- set to null update");
                        sbUpdates.AppendFormat(BuildForeignUpdateSql(
                            propertyValue.Property.ForeignEntity.Table,
                            entityRecord.Entity.Keys.FirstOrDefault().Column,
                            (paramIndex++).ToString(),
                            wherePart2));
                        cmd.AddParam(null);
                    }
                }

                var values =
                    propertyValue.Values.Select(
                        x => x.ToStringSafe().Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                var whereParts = new List<string>();
                var addSqlPart = true;
                for (int i = 0; i < propertyValue.Property.ForeignEntity.Keys.Count; i++)
                {
                    var key = propertyValue.Property.ForeignEntity.Keys[i];
                    var joinedValues = string.Join(",", values.Select(x => "@" + paramIndex++));
                    whereParts.Add("{0} In ({1})".Fill(key.Column, joinedValues));
                    addSqlPart = joinedValues.HasValue();
                    if (addSqlPart == false)
                        break;
                    cmd.AddParams(values.Select(x => x[i]).OfType<object>().ToArray());
                }
                if (addSqlPart)
                {
                    var wherePart = string.Join(" AND ", whereParts);
                    sbUpdates.AppendLine();
                    sbUpdates.Append(BuildForeignUpdateSql(
                        propertyValue.Property.ForeignEntity.Table,
                        entityRecord.Entity.Keys.FirstOrDefault().Column,
                        (paramIndex++).ToString(),
                        wherePart));
                    cmd.AddParam(entityRecord.Keys.FirstOrDefault().Raw);
                }
            }

            cmd.CommandText += Environment.NewLine + "-- update foreign entities records" + sbUpdates.ToString();
        }

        private void AddManyToManyForeignsUpdate(DbCommand cmd, EntityRecord entityRecord)
        {
            if (entityRecord.Keys.Count > 1)
                return;
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var propertyValue in entityRecord.Values.WhereOneToMany()
                .Where(x => x.Property.IsManyToMany))
            {
                var recordKey = entityRecord.Keys.FirstOrDefault().AsString;
                var actualRecords = _source.GetRecords(
                    propertyValue.Property.ForeignEntity,
                    new List<BaseFilter>
                    {
                        new ForeignEntityFilter(
                            entityRecord.Entity.Keys.FirstOrDefault(),
                            recordKey)
                    }).Records;

                var selectedValues = propertyValue.Values.Select(x => x.ToStringSafe()).ToList();

                var mtmEntity = GetEntityToLoad(propertyValue.Property);
                var dbIds = actualRecords
                    .Select(x => x.Keys.FirstOrDefault(y => y.Property.ForeignEntity == mtmEntity).AsString)
                    .ToList();
                var idsToRemove = dbIds
                    .Except(selectedValues)
                    .ToList();
                if (idsToRemove.Any())
                {
                    sbUpdates.AppendLine();
                    sbUpdates.AppendLine("-- delete many to many records");
                    foreach (var idToRemove in idsToRemove)
                    {
                        var foreignEntity = propertyValue.Property.ForeignEntity;
                        var key1 =
                            foreignEntity.ForeignKeys.FirstOrDefault(
                                x => x.ForeignEntity == propertyValue.Property.Entity);
                        var key2 =
                            foreignEntity.ForeignKeys.FirstOrDefault(
                                x => x.ForeignEntity == mtmEntity);
                        cmd.AddParam(recordKey);
                        cmd.AddParam(idToRemove);
                        sbUpdates.AppendLine($"DELETE {foreignEntity.Table} WHERE {key1.Column} = @{paramIndex++} and {key2.Column} = @{paramIndex++}");
                    }
                }

                var idsToAdd = selectedValues
                    .Except(dbIds)
                    .ToList();
                if (idsToAdd.Any())
                {
                    sbUpdates.AppendLine();
                    sbUpdates.AppendLine("-- add many to many records");
                    foreach (var idToAdd in idsToAdd)
                    {
                        var foreignEntity = propertyValue.Property.ForeignEntity;
                        var key1 =
                            foreignEntity.ForeignKeys.FirstOrDefault(
                                x => x.ForeignEntity == propertyValue.Property.Entity);
                        var key2 =
                            foreignEntity.ForeignKeys.FirstOrDefault(
                                x => x.ForeignEntity == mtmEntity);
                        cmd.AddParam(recordKey);
                        cmd.AddParam(idToAdd);
                        sbUpdates.AppendLine($"INSERT INTO {foreignEntity.Table} ({key1.Column}, {key2.Column}) VALUES(@{paramIndex++}, @{paramIndex++})");
                    }
                }
            }

            cmd.CommandText += Environment.NewLine + sbUpdates;
        }

        private static Entity GetEntityToLoad(Property foreignProperty)
        {
            if (foreignProperty.IsManyToMany)
            {
                return foreignProperty.ForeignEntity.ForeignKeys
                    .First(x => x.ForeignEntity != foreignProperty.Entity)
                    .ForeignEntity;
            }

            return foreignProperty.ForeignEntity;
        }

        private string BuildForeignUpdateSql(
            string table,
            string foreignKey,
            string foreignValueSqlParameterName,
            string constraints)
        {
            return $@"UPDATE {table} 
   SET {foreignKey} = @{foreignValueSqlParameterName} 
 WHERE {constraints};";
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
                    case ValueBehavior.Guid:
                        cmd.AddParam(Guid.NewGuid());
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