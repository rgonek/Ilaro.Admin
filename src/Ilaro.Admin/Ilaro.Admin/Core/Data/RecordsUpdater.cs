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

        public bool Update(EntityRecord entityRecord, Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord);

                var result = _executor.ExecuteWithChanges(
                    cmd,
                    entityRecord,
                    EntityChangeType.Update,
                    changeDescriber);

                return result != null;
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
            if (entityRecord.Key.Count == 1)
                AddForeignsUpdate(cmd, entityRecord);

            return cmd;
        }

        protected virtual DbCommand CreateBaseCommand(EntityRecord entityRecord)
        {
            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            var updateProperties = entityRecord.Values
                .WhereIsNotSkipped()
                .WhereIsNotOneToMany()
                .Where(value => value.Property.IsKey == false)
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
                cmd.AddParams(entityRecord.Key.Select(value => value.Raw).ToArray());
                var setsSeparator = "," + Environment.NewLine + "       ";
                var sets = string.Join(setsSeparator, setsList);
                var whereParts = new List<string>();
                foreach (var key in entityRecord.Key)
                {
                    var column = key.Property.Column;
                    var parameterName = (counter++).ToString();
                    whereParts.Add($"{key.Property.Column} = @{parameterName}");
                }
                var constraintSeparator = Environment.NewLine + "   AND ";
                var constraints = string.Join(constraintSeparator, whereParts);
                var table = entityRecord.Entity.Table;
                cmd.CommandText = $@"-- update record
UPDATE {table}
   SET {sets} 
 WHERE {constraints};
";
            }
            cmd.AddParam(entityRecord.JoinedKeyValue);
            var joinedKeyValueParameterName = counter.ToString();
            cmd.CommandText += $@"-- return record id
SELECT @{joinedKeyValueParameterName};
-- update foreign entities records";

            return cmd;
        }

        private void AddForeignsUpdate(DbCommand cmd, EntityRecord entityRecord)
        {
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var propertyValue in entityRecord.Values.WhereOneToMany())
            {
                var actualRecords = _source.GetRecords(
                    propertyValue.Property.ForeignEntity,
                    new List<BaseFilter>
                    {
                        new ForeignEntityFilter(
                            entityRecord.Entity.Key.FirstOrDefault(),
                            entityRecord.Key.FirstOrDefault().Raw.ToStringSafe())
                    }).Records;
                var idsToRemoveRelation = actualRecords
                    .Select(x => x.JoinedKeyValue)
                    .Except(propertyValue.Values.Select(x => x.ToStringSafe()))
                    .ToList();
                if (idsToRemoveRelation.Any())
                {
                    var values2 =
                        idsToRemoveRelation.Select(
                            x => x.Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                    var whereParts2 = new List<string>();
                    for (int i = 0; i < propertyValue.Property.ForeignEntity.Key.Count; i++)
                    {
                        var key = propertyValue.Property.ForeignEntity.Key[i];
                        var joinedValues = string.Join(",", values2.Select(x => "@" + paramIndex++));
                        whereParts2.Add("{0} In ({1})".Fill(key.Column, joinedValues));
                        cmd.AddParams(values2.Select(x => x[i]).OfType<object>().ToArray());
                    }
                    var wherePart2 = string.Join(" AND ", whereParts2);
                    sbUpdates.AppendLine();
                    sbUpdates.AppendLine("-- set to null update");
                    sbUpdates.AppendFormat(BuildForeignUpdateSql(
                        propertyValue.Property.ForeignEntity.Table,
                        entityRecord.Entity.Key.FirstOrDefault().Column,
                        (paramIndex++).ToString(),
                        wherePart2));
                    cmd.AddParam(null);
                }

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
                sbUpdates.Append(BuildForeignUpdateSql(
                    propertyValue.Property.ForeignEntity.Table,
                    entityRecord.Entity.Key.FirstOrDefault().Column,
                    (paramIndex++).ToString(),
                    wherePart));
                cmd.AddParam(entityRecord.Key.FirstOrDefault().Raw);
            }

            cmd.CommandText += sbUpdates.ToString();
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