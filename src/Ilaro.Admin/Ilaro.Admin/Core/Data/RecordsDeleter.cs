using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Massive;
using System.Data;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsDeleter : IDeletingRecords
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsDeleter));
        private readonly IIlaroAdmin _admin;
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecordsHierarchy _hierarchySource;
        private readonly IProvidingUser _user;

        public RecordsDeleter(
            IIlaroAdmin admin,
            IExecutingDbCommand executor,
            IFetchingRecordsHierarchy hierarchySource,
            IProvidingUser user)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));
            if (hierarchySource == null)
                throw new ArgumentNullException(nameof(hierarchySource));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _admin = admin;
            _executor = executor;
            _hierarchySource = hierarchySource;
            _user = user;
        }

        public bool Delete(
            EntityRecord entityRecord,
            IDictionary<string, PropertyDeleteOption> options,
            Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord, options);

                var result = _executor.ExecuteWithChanges(
                    cmd,
                    entityRecord,
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

        private DbCommand CreateCommand(
            EntityRecord entityRecord,
            IDictionary<string, PropertyDeleteOption> options)
        {
            var cmd = CreateBaseCommand(entityRecord);
            if (entityRecord.Entity.SoftDeleteEnabled == false)
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
            var constraintsSeparator = Environment.NewLine + "   AND ";
            var constraints = string.Join(constraintsSeparator, whereParts);
            cmd.AddParam(entityRecord.JoinedKeyValue);
            var joinedKeySqlParamName = (counter++).ToString();
            var table = entityRecord.Entity.Table;

            if (entityRecord.Entity.SoftDeleteEnabled)
            {
                var setsList = new List<string>();
                foreach (var property in entityRecord.Entity.Properties.
                    Where(x => x.OnDeleteDefaultValue != null))
                {
                    AddParam(cmd, property);
                    var column = property.Column;
                    var parameterName = (counter++).ToString();
                    setsList.Add($"{column} = @{parameterName}");
                }
                var setSeparator = "," + Environment.NewLine + "       ";
                var sets = string.Join(setSeparator, setsList);
                cmd.CommandText =
$@"UPDATE {table}
   SET {sets}
 WHERE {constraints};

SELECT @{joinedKeySqlParamName};";
            }
            else
            {
                cmd.CommandText =
$@"DELETE {table}
 WHERE {constraints};

SELECT @{joinedKeySqlParamName};";
            }

            return cmd;
        }

        private void AddParam(DbCommand cmd, Property property)
        {
            if (property.OnDeleteDefaultValue is ValueBehavior)
            {
                switch (property.OnDeleteDefaultValue as ValueBehavior?)
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
                if (property.TypeInfo.IsFileStoredInDb)
                    cmd.AddParam(property.OnDeleteDefaultValue, DbType.Binary);
                else
                    cmd.AddParam(property.OnDeleteDefaultValue);
            }
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
            IDictionary<string, PropertyDeleteOption> options)
        {
            if (options.Where(x => x.Value.Level == 0)
                .All(x => x.Value.DeleteOption == CascadeOption.Nothing ||
                    x.Value.DeleteOption == CascadeOption.AskUser))
                return;

            var entityHierarchy = _hierarchySource.GetEntityHierarchy(entityRecord.Entity);
            var keyConstraint = BuildKeyConstraint(entityRecord, entityHierarchy.Alias);
            var sqlNullParameterName = "";
            if (options.Any(x => x.Value.DeleteOption == CascadeOption.Detach))
            {
                sqlNullParameterName = cmd.Parameters.Count.ToString();
                cmd.AddParam(null);
            }

            var sqlsBuilder = new StringBuilder();
            GetForeignSql(
                entityHierarchy,
                options,
                sqlsBuilder,
                keyConstraint,
                sqlNullParameterName);

            cmd.CommandText = sqlsBuilder + cmd.CommandText;
        }

        private void GetForeignSql(
            EntityHierarchy entityHierarchy,
            IDictionary<string, PropertyDeleteOption> options,
            StringBuilder sqlsBuilder,
            string keyConstraint,
            string sqlNullParameterName,
            string hierarchyPrefix = null,
            int level = 0)
        {
            if (options.Where(x => x.Value.Level == level)
                .All(x => x.Value.DeleteOption == CascadeOption.Nothing ||
                    x.Value.DeleteOption == CascadeOption.AskUser))
                return;

            if (hierarchyPrefix.HasValue())
                hierarchyPrefix += "-";

            foreach (var subHierarchy in entityHierarchy.SubHierarchies)
            {
                var deleteOption = CascadeOption.Nothing;
                if (options.ContainsKey(hierarchyPrefix + subHierarchy.Entity.Name))
                {
                    deleteOption = options[hierarchyPrefix + subHierarchy.Entity.Name].DeleteOption;
                }

                switch (deleteOption)
                {
                    case CascadeOption.Detach:
                        sqlsBuilder.AppendLine(GetSetNullUpdateSql(
                            subHierarchy,
                            sqlNullParameterName,
                            keyConstraint));
                        break;
                    case CascadeOption.Delete:
                        GetForeignSql(
                            subHierarchy,
                            options,
                            sqlsBuilder,
                            keyConstraint,
                            sqlNullParameterName,
                            hierarchyPrefix + subHierarchy.Entity.Name,
                            ++level);
                        sqlsBuilder.AppendLine(GetRelatedEntityDeleteSql(
                            subHierarchy,
                            keyConstraint));
                        break;
                }
            }
        }

        private string GetRelatedEntityDeleteSql(
            EntityHierarchy hierarchy,
            string keyConstraint)
        {
            var join = GetJoin(hierarchy, hierarchy.ParentHierarchy);
            var table = hierarchy.Entity.Table;
            var alias = hierarchy.Alias;

            var delete =
$@"DELETE {alias}
  FROM {table} {alias}{join}
 WHERE {keyConstraint};
";

            return delete;
        }

        private string GetJoin(EntityHierarchy hierarchy, EntityHierarchy parentHierarchy)
        {
            var parentTable = parentHierarchy.Entity.Table;
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
            EntityHierarchy hierarchy,
            string sqlNullParameterName,
            string keyConstraint)
        {
            var table = hierarchy.Entity.Table;
            var alias = hierarchy.Alias;
            var sets = new List<string>();

            foreach (var foreignKey in hierarchy.Entity.Properties
                .Where(x => x.ForeignEntity == hierarchy.ParentHierarchy.Entity))
            {
                var key = hierarchy.ParentHierarchy.Entity.Key
                    .FirstOrDefault(x => x.Name == foreignKey.ReferencePropertyName);
                if (key != null)
                {
                    sets.Add($"{alias}.{foreignKey.Column} = @{sqlNullParameterName}");
                }
            }

            var setSeparator = "," + Environment.NewLine + "       ";
            var constraintSeparator = Environment.NewLine + "   AND ";
            var set = string.Join(setSeparator, sets);
            var join = GetJoin(hierarchy, hierarchy.ParentHierarchy);

            var updateSql =
$@"UPDATE {alias}
   SET {set}
  FROM {table} {alias}{join}
 WHERE {keyConstraint};
";

            return updateSql;
        }
    }
}