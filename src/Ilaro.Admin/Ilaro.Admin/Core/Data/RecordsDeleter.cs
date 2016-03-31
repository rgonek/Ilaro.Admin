using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsDeleter : IDeletingRecords
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsDeleter));
        private readonly IIlaroAdmin _admin;
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecordsHierarchy _hierarchySource;

        private const string SqlFormat =
@"DELETE FROM {0} WHERE {1};

SELECT @{2};";

        public RecordsDeleter(
            IIlaroAdmin admin,
            IExecutingDbCommand executor,
            IFetchingRecordsHierarchy hierarchySource)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));
            if (hierarchySource == null)
                throw new ArgumentNullException(nameof(hierarchySource));

            _admin = admin;
            _executor = executor;
            _hierarchySource = hierarchySource;
        }

        public bool Delete(
            EntityRecord entityRecord,
            IDictionary<string, DeleteOption> options,
            Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord, options);

                var result = _executor.ExecuteWithChanges(
                    cmd,
                    entityRecord.Entity.Name,
                    EntityChangeType.Delete,
                    changeDescriber);

                return result != null;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        private DbCommand CreateCommand(EntityRecord entityRecord, IDictionary<string, DeleteOption> options)
        {
            var cmd = CreateBaseCommand(entityRecord);
            AddForeignsSql(cmd, entityRecord, options);

            return cmd;
        }

        private DbCommand CreateBaseCommand(EntityRecord entityRecord)
        {
            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var whereParts = new List<string>();
            var counter = 0;
            foreach (var key in entityRecord.Key)
            {
                key.SqlParameterName = (counter++).ToString();
                whereParts.Add("{0} = @{1}".Fill(key.Property.Column, key.SqlParameterName));
                cmd.AddParam(key.Raw);
            }
            var wherePart = string.Join(" AND ", whereParts);
            cmd.AddParam(entityRecord.JoinedKeyValue);
            cmd.CommandText = SqlFormat.Fill(entityRecord.Entity.TableName, wherePart, counter);

            return cmd;
        }

        private string BuildKeyConstraint(EntityRecord entityRecord, string alias)
        {
            var constraints = new List<string>();
            foreach (var key in entityRecord.Key)
            {
                var column = key.Property.Column;
                var sqlParameter = key.SqlParameterName;
                constraints.Add($"{alias}.{column} = @{sqlParameter}");
            }
            var constraint = string.Join(" AND ", constraints);

            return constraint;
        }

        private void AddForeignsSql(
            DbCommand cmd,
            EntityRecord entityRecord,
            IDictionary<string, DeleteOption> options)
        {
            if (options.All(x => x.Value == DeleteOption.Nothing || x.Value == DeleteOption.AskUser))
                return;

            var sqls = new StringBuilder();
            var entityHierarchy = _hierarchySource.GetEntityHierarchy(entityRecord.Entity);
            var keyConstraint = BuildKeyConstraint(entityRecord, entityHierarchy.Alias);
            var sqlNullParameterName = "";
            if (options.Any(x => x.Value == DeleteOption.SetNull))
            {
                sqlNullParameterName = cmd.Parameters.Count.ToString();
                cmd.AddParam(null);
            }
            foreach (var subHierarchy in entityHierarchy.SubHierarchies)
            {
                var deleteOption = DeleteOption.Nothing;
                if (options.ContainsKey(subHierarchy.Entity.Name))
                {
                    deleteOption = options[subHierarchy.Entity.Name];
                }

                switch (deleteOption)
                {
                    case DeleteOption.SetNull:
                        sqls.AppendLine(GetSetNullUpdateSql(
                            entityRecord,
                            subHierarchy,
                            sqlNullParameterName));
                        break;
                    case DeleteOption.CascadeDelete:
                        sqls.AppendLine(GetRelatedEntityDeleteSql(
                            subHierarchy,
                            keyConstraint));
                        break;
                }
            }

            cmd.CommandText = sqls + cmd.CommandText;
        }

        private string GetRelatedEntityDeleteSql(
            EntityHierarchy hierarchy,
            string keyConstraint)
        {
            var delete = "";
            foreach (var subHierarchy in hierarchy.SubHierarchies)
            {
                delete +=
                    GetRelatedEntityDeleteSql(subHierarchy, keyConstraint) +
                    Environment.NewLine;
            }

            var join = GetJoin(hierarchy, hierarchy.ParentHierarchy);
            var table = hierarchy.Entity.TableName;
            var alias = hierarchy.Alias;

            delete +=
$@"DELETE {alias}
  FROM {table} {alias}{join}
 WHERE {keyConstraint};
";

            return delete;
        }

        private string GetJoin(EntityHierarchy hierarchy, EntityHierarchy parentHierarchy)
        {
            var parentTable = parentHierarchy.Entity.TableName;
            var parentAlias = parentHierarchy.Alias;
            string parentKey, key;
            var alias = hierarchy.Alias;

            var foreignProperty = parentHierarchy.Entity.ForeignKey.FirstOrDefault(x => x.ForeignEntity == hierarchy.Entity);
            if (foreignProperty == null || foreignProperty.TypeInfo.IsCollection)
            {
                parentKey = parentHierarchy.Entity.Key.FirstOrDefault().Column;
                key = hierarchy.Entity.ForeignKey
                    .FirstOrDefault(x => x.ForeignEntity == parentHierarchy.Entity).Column;
            }
            else
            {
                parentKey = foreignProperty.Column;
                key = hierarchy.Entity.Key.FirstOrDefault().Column;
            }

            var join =
$@"
  LEFT OUTER JOIN {parentTable} AS {parentAlias} ON {parentAlias}.{parentKey} = {alias}.{key}";

            return parentHierarchy.ParentHierarchy != null ?
                join + GetJoin(parentHierarchy, parentHierarchy.ParentHierarchy) :
                join;
        }

        private string GetSetNullUpdateSql(
            EntityRecord entityRecord,
            EntityHierarchy hierarchy,
            string sqlNullParameterName)
        {
            var table = hierarchy.Entity.TableName;
            var alias = hierarchy.Alias;
            var sets = new List<string>();
            var constraints = new List<string>();

            foreach (var foreignKey in hierarchy.Entity.Properties
                .Where(x => x.ForeignEntity == entityRecord.Entity))
            {
                var key = entityRecord.Key
                    .FirstOrDefault(x => x.Property.Name == foreignKey.ReferencePropertyName);
                if (key != null)
                {
                    sets.Add($"{alias}.{foreignKey.Column} = @{sqlNullParameterName}");
                    constraints.Add($"{alias}.{foreignKey.Column} =  @{key.SqlParameterName}");
                }
            }

            var setSeparator = "," + Environment.NewLine + "       ";
            var constraintSeparator = Environment.NewLine + "   AND ";
            var set = string.Join(setSeparator, sets);
            var keyConstraint = string.Join(constraintSeparator, constraints);

            var updateSql =
$@"UPDATE {alias}
   SET {set}
  FROM {table} {alias}
 WHERE {keyConstraint};
";

            return updateSql;
        }
    }
}