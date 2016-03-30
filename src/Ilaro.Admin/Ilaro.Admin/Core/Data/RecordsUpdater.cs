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
                throw new ArgumentNullException(nameof(admin));
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            _admin = admin;
            _executor = executor;
            _source = source;
        }

        public bool Update(EntityRecord entityRecord, Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord);

                // TODO: get info about changed properties
                var result = _executor.ExecuteWithChanges(
                    cmd, 
                    entityRecord.Entity.Name, 
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
            var sbKeys = new StringBuilder();

            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            var updateProperties = entityRecord.Values
                .WhereIsNotSkipped()
                .WhereIsNotOneToMany()
                .Where(value => value.Property.IsKey == false)
                .ToList();
            if (updateProperties.Any())
            {
                foreach (var propertyValue in updateProperties)
                {
                    AddParam(cmd, propertyValue);
                    sbKeys.AppendFormat("\t{0} = @{1}, \r\n", propertyValue.Property.Column, counter++);
                }
                cmd.AddParams(entityRecord.Key.Select(value => value.Raw).ToArray());
                var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 4);
                var whereParts = new List<string>();
                foreach (var key in entityRecord.Key)
                {
                    whereParts.Add("{0} = @{1}".Fill(key.Property.Column, counter++));
                }
                var wherePart = string.Join(" AND ", whereParts);
                cmd.CommandText = SqlFormat.Fill(entityRecord.Entity.TableName, keys, wherePart);
            }
            cmd.AddParam(entityRecord.JoinedKeyValue);
            cmd.CommandText += SqlReturnRecordIdPart.Fill(counter);

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
                    sbUpdates.AppendFormat(
                        RelatedRecordsUpdateSqlFormat,
                        propertyValue.Property.ForeignEntity.TableName,
                        entityRecord.Entity.Key.FirstOrDefault().Column,
                        paramIndex++,
                        wherePart2);
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
                sbUpdates.AppendFormat(
                    RelatedRecordsUpdateSqlFormat,
                    propertyValue.Property.ForeignEntity.TableName,
                    entityRecord.Entity.Key.FirstOrDefault().Column,
                    paramIndex++,
                    wherePart);
                cmd.AddParam(entityRecord.Key.FirstOrDefault().Raw);
            }

            cmd.CommandText += sbUpdates.ToString();
        }

        private static void AddParam(DbCommand cmd, PropertyValue propertyValue)
        {
            if (propertyValue.Property.TypeInfo.IsFileStoredInDb)
                cmd.AddParam(propertyValue.Raw, DbType.Binary);
            else
            {
                if (propertyValue.Raw.IsBehavior(DefaultValueBehavior.Now) ||
                    propertyValue.Raw.IsBehavior(DefaultValueBehavior.NowOnUpdate))
                {
                    cmd.AddParam(DateTime.Now);
                }
                else if (propertyValue.Raw.IsBehavior(DefaultValueBehavior.UtcNow) ||
                    propertyValue.Raw.IsBehavior(DefaultValueBehavior.UtcNowOnUpdate))
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