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
        private readonly IExecutingDbCommand _executor;
        private readonly IFetchingRecordsHierarchy _hierarchySource;

        private const string SqlFormat =
@"DELETE FROM {0} WHERE {1};
SELECT @{2};";

        public RecordsDeleter(
            IExecutingDbCommand executor,
            IFetchingRecordsHierarchy hierarchySource)
        {
            if (executor == null)
                throw new ArgumentNullException("executor");
            if (hierarchySource == null)
                throw new ArgumentNullException("hierarchySource");

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
                _log.Error(ex);
                throw;
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
            var whereParts = new List<string>();
            var counter = 0;
            foreach (var key in entity.Key)
            {
                whereParts.Add("{0} = @{1}".Fill(key.ColumnName, counter++));
                cmd.AddParam(key.Value.Raw);
            }
            var wherePart = string.Join(" AND ", whereParts);
            cmd.AddParam(entity.JoinedKeyValue);
            cmd.CommandText = SqlFormat.Fill(entity.TableName, wherePart, counter);

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
            const string deleteFormat = "DELETE FROM {0} WHERE {1};";

            var paramIndex = cmd.Parameters.Count;
            cmd.AddParams(record.KeyValue.ToArray());
            var whereParts = new List<string>();
            foreach (var key in record.Entity.Key)
            {
                whereParts.Add("{0} = @{1}".Fill(key.ColumnName, paramIndex++));
            }
            var wherePart = string.Join(" AND ", whereParts);

            var sql = deleteFormat.Fill(
                record.Entity.TableName,
                wherePart);

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
            const string updateFormat = "UPDATE {0} SET {1} = @{2} WHERE {3};";
            //UPDATE Products SET CategoryID = null WHERE ProductID = 7

            var foreignTable = subRecord.Entity.TableName;
            var foreignKey = subRecord.Entity.Properties.FirstOrDefault(x => x.ForeignEntity == entity).ColumnName;
            var nullIndex = cmd.Parameters.Count;
            var paramIndex = nullIndex + 1;
            cmd.AddParam(null);
            cmd.AddParams(subRecord.KeyValue.ToArray());

            var whereParts = new List<string>();
            foreach (var key in subRecord.Entity.Key)
            {
                whereParts.Add("{0} = @{1}".Fill(key.ColumnName, paramIndex++));
            }
            var wherePart = string.Join(" AND ", whereParts);

            var updateSql = updateFormat.Fill(
                foreignTable,
                foreignKey,
                nullIndex,
                wherePart);

            return updateSql;
        }
    }
}