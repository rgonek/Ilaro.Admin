using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Extensions2;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsDeleter : IDeletingRecords
    {
        private readonly Notificator _notificator;
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecordsHierarchy _hierarchySource;

        private const string SqlFormat =
@"DELETE FROM {0} WHERE {1} = @0;
SELECT @0;";

        public RecordsDeleter(
            Notificator notificator,
            IExecutingDbCommand executor,
            IFetchingRecordsHierarchy hierarchySource)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (executor == null)
                throw new ArgumentNullException("executor");
            if (hierarchySource == null)
                throw new ArgumentNullException("hierarchySource");

            _notificator = notificator;
            _executor = executor;
            _hierarchySource = hierarchySource;
        }

        public bool Delete(Entity entity, IDictionary<string, DeleteOption> options)
        {
            try
            {
                var cmd = CreateCommand(entity, options);

                var result = (int)_executor
                    .ExecuteWithChanges(cmd, new ChangeInfo(entity.Name, EntityChangeType.Delete));

                return result > 0;
            }
            catch (Exception ex)
            {
                _notificator.Error("");
                return false;
            }
            finally
            {
                entity.ClearPropertiesValues();
            }
        }

        private DbCommand CreateCommand(Entity entity, IDictionary<string, DeleteOption> options)
        {
            var cmd = CreateBaseCommand(entity);
            AddForeignsDelete(cmd, entity, options);

            return cmd;
        }

        private DbCommand CreateBaseCommand(Entity entity)
        {
            var cmd = DB.CreateCommand();
            cmd.CommandText = SqlFormat.Fill(entity.TableName, entity.Key.ColumnName);
            cmd.AddParam(entity.Key.Value.Raw);

            return cmd;
        }

        private void AddForeignsDelete(
            DbCommand cmd,
            Entity entity,
            IDictionary<string, DeleteOption> options)
        {
            if (options.All(x => x.Value == DeleteOption.Nothing || x.Value == DeleteOption.AskUser))
                return;

            var sbDeletes = new StringBuilder();
            var recordHierarchy = _hierarchySource.GetRecordHierarchy(entity);
            foreach (var subRecord in recordHierarchy.SubRecordsHierarchies)
            {
                var deleteOption = DeleteOption.Nothing;
                if (options.ContainsKey(subRecord.Entity.Name))
                {
                    deleteOption = options[subRecord.Entity.Name];
                }
                switch (deleteOption)
                {
                    case DeleteOption.SetNull:
                        sbDeletes.AppendLine(GetSetToNullUpdateSql(cmd, entity, subRecord));
                        break;
                    case DeleteOption.CascadeDelete:
                        var deletes = GetDeleteRelatedEntityDeleteSql(cmd, subRecord).Reverse();
                        sbDeletes.AppendLine(string.Join(Environment.NewLine, deletes));
                        break;
                }
            }
            cmd.CommandText = sbDeletes + cmd.CommandText;
        }

        private IList<string> GetDeleteRelatedEntityDeleteSql(DbCommand cmd, RecordHierarchy record)
        {
            // {0} - Foreign table
            // {1} - Primary key
            // {2} - Key value
            const string deleteFormat = "DELETE FROM {0} WHERE {1} = @{2};";

            var paramIndex = cmd.Parameters.Count;
            cmd.AddParam(record.KeyValue);

            var sql = deleteFormat.Fill(
                record.Entity.TableName,
                record.Entity.Key.ColumnName,
                paramIndex);

            var sqls = new List<string>() { sql };
            foreach (var subRecord in record.SubRecordsHierarchies)
            {
                sqls.AddRange(GetDeleteRelatedEntityDeleteSql(cmd, subRecord));
            }

            return sqls;
        }

        private string GetSetToNullUpdateSql(DbCommand cmd, Entity entity, RecordHierarchy subRecord)
        {
            // {0} - Foreign table
            // {1} - Foreign key
            // {2} - Primary key
            // {3} - Key value
            const string updateFormat = "UPDATE {0} SET {1} = @{2} WHERE {3} = @{4};";
            //UPDATE Products SET CategoryID = null WHERE ProductID = 7

            var foreignTable = subRecord.Entity.TableName;
            var foreignKey = subRecord.Entity.Properties.FirstOrDefault(x => x.ForeignEntity == entity).ColumnName;
            var primaryKey = subRecord.Entity.Key.ColumnName;
            var paramIndex = cmd.Parameters.Count;
            cmd.AddParam(null);
            cmd.AddParam(subRecord.KeyValue);

            var updateSql = updateFormat.Fill(
                foreignTable,
                foreignKey,
                paramIndex++,
                primaryKey,
                paramIndex);

            return updateSql;
        }
    }
}