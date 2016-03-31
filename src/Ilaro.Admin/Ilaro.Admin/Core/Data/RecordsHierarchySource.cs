using System;
using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Models;
using Massive;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsHierarchySource : IFetchingRecordsHierarchy
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsHierarchySource));
        private readonly IIlaroAdmin _admin;

        public RecordsHierarchySource(IIlaroAdmin admin)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));

            _admin = admin;
        }

        public RecordHierarchy GetRecordHierarchy(EntityRecord entityRecord)
        {
            _log.InfoFormat(
                "Getting record hierarchy for entity record ({0}#{1})",
                entityRecord.Entity.Name,
                entityRecord.JoinedKeyWithValue);

            var hierarchy = GetEntityHierarchy(entityRecord.Entity);
            var sql = GenerateHierarchySql(hierarchy);
            _log.DebugFormat("Sql hierarchy: \r\n {0}", sql);
            var model = new DynamicModel(_admin.ConnectionStringName);
            var records = model.Query(sql, entityRecord.Key.Select(x => x.Raw).ToArray()).ToList();

            var recordHierarchy = GetHierarchyRecords(records, hierarchy);

            return recordHierarchy;
        }

        public EntityHierarchy GetEntityHierarchy(Entity entity)
        {
            var index = 0;
            return GetEntityHierarchy(null, entity, ref index);
        }

        private RecordHierarchy GetHierarchyRecords(
            IList<IDictionary<string, object>> records,
            EntityHierarchy hierarchy)
        {
            var baseRecord = records.FirstOrDefault();
            var prefix = hierarchy.Alias.Undecorate() + "_";
            var rowData = new DataRow(baseRecord, hierarchy.Entity, prefix);

            var recordHierarchy = new RecordHierarchy
            {
                Entity = hierarchy.Entity,
                KeyValue = rowData.KeyValue,
                DisplayName = rowData.ToString(hierarchy.Entity),
                SubRecordsHierarchies = new List<RecordHierarchy>()
            };

            GetHierarchyRecords(recordHierarchy, records, hierarchy.SubHierarchies);

            return recordHierarchy;
        }

        private void GetHierarchyRecords(
            RecordHierarchy parentHierarchy,
            IList<IDictionary<string, object>> records,
            IEnumerable<EntityHierarchy> subHierarchies,
            IList<string> foreignKey = null,
            IList<string> foreignKeyValue = null)
        {
            foreach (var hierarchy in subHierarchies)
            {
                var prefix = hierarchy.Alias.Undecorate() + "_";

                foreach (var record in records)
                {
                    var rowData = new DataRow(record, hierarchy.Entity, prefix);

                    if (!rowData.KeyValue.IsNullOrEmpty())
                    {
                        var subRecord = new RecordHierarchy
                        {
                            Entity = hierarchy.Entity,
                            KeyValue = rowData.KeyValue,
                            DisplayName = rowData.ToString(hierarchy.Entity),
                            SubRecordsHierarchies = new List<RecordHierarchy>()
                        };

                        if (parentHierarchy.SubRecordsHierarchies.FirstOrDefault(x => x.JoinedKeyValue == subRecord.JoinedKeyValue) == null &&
                            Matching(record, foreignKey, foreignKeyValue))
                        {
                            parentHierarchy.SubRecordsHierarchies.Add(subRecord);

                            GetHierarchyRecords(
                                subRecord,
                                records,
                                hierarchy.SubHierarchies,
                                hierarchy.Entity.Key.Select(x => prefix + x.Column.Undecorate()).ToList(),
                                rowData.KeyValue);
                        }
                    }
                }
            }
        }

        private bool Matching(IDictionary<string, object> recordDict, IList<string> foreignKey, IList<string> foreignKeyValue)
        {
            if (foreignKey == null || foreignKey.Count == 0 || foreignKey.All(x => x.IsNullOrWhiteSpace()))
            {
                return true;
            }

            if (foreignKey.Count != foreignKeyValue.Count)
                return false;

            for (int i = 0; i < foreignKey.Count; i++)
            {
                if (recordDict[foreignKey[i]].ToStringSafe() != foreignKeyValue[i])
                    return false;
            }

            return true;
        }

        private string GenerateHierarchySql(EntityHierarchy hierarchy)
        {
            var flatHierarchy = FlatHierarchy(hierarchy);

            // {0} - Columns
            // {1} - Base table
            // {2} - Base table alias
            // {3} - Joins
            // {4} - Where
            // {5} - Order by
            const string selectFormat =
@"SELECT {0} 
FROM {1} AS {2}
{3}
WHERE {4}
ORDER BY {5};";
            // {0} - Foreign table
            // {1} - Foreign alias
            // {2} - Foreign key
            // {3} - Base table alias
            // {4} - Base table primary key
            const string joinFormat = @"LEFT OUTER JOIN {0} AS {1} ON {1}.{2} = {3}.{4}";

            var columns = flatHierarchy.SelectMany(x => x.Entity.DisplayProperties.Select(y => y.Column).Distinct()
                .Select(y => x.Alias + "." + y + " AS " + x.Alias.Undecorate() + "_" + y.Undecorate())).ToList();
            var joins = new List<string>();
            foreach (var item in flatHierarchy.Where(x => x.ParentHierarchy != null))
            {
                var foreignTable = item.Entity.TableName;
                var foreignAlias = item.Alias;
                string foreignKey, baseTablePrimaryKey;
                var baseTableAlias = item.ParentHierarchy.Alias;
                var foreignProperty = item.Entity.Properties.FirstOrDefault(x => x.ForeignEntity == item.ParentHierarchy.Entity);
                if (foreignProperty == null || foreignProperty.TypeInfo.IsCollection)
                {
                    foreignKey = item.Entity.Key.FirstOrDefault().Column;
                    baseTablePrimaryKey = item.ParentHierarchy.Entity.Properties
                        .FirstOrDefault(x => x.ForeignEntity == item.Entity).Column;
                }
                else
                {
                    foreignKey = foreignProperty.Column;
                    baseTablePrimaryKey = item.ParentHierarchy.Entity.Key.FirstOrDefault().Column;
                }
                joins.Add(joinFormat.Fill(
                    foreignTable,
                    foreignAlias,
                    foreignKey,
                    baseTableAlias,
                    baseTablePrimaryKey));
            }
            var orders = flatHierarchy.SelectMany(x => x.Entity.Key.Select(y => x.Alias + "." + y.Column)).ToList();

            var whereParts = new List<string>();
            var counter = 0;
            foreach (var key in hierarchy.Entity.Key)
            {
                whereParts.Add("{0}.{1} = @{2}".Fill(hierarchy.Alias, key.Column, counter++));
            }
            var where = string.Join(" AND ", whereParts);

            var sql = selectFormat.Fill(
                string.Join(", ", columns),
                hierarchy.Entity.TableName,
                hierarchy.Alias,
                string.Join(Environment.NewLine, joins),
                where,
                string.Join(", ", orders));

            return sql;
        }

        private IList<EntityHierarchy> FlatHierarchy(EntityHierarchy hierarchy)
        {
            var flatHierarchy = new List<EntityHierarchy> { hierarchy };

            foreach (var subHierarchy in hierarchy.SubHierarchies)
            {
                flatHierarchy.AddRange(FlatHierarchy(subHierarchy));
            }

            return flatHierarchy;
        }

        private EntityHierarchy GetEntityHierarchy(
            EntityHierarchy parent,
            Entity entity,
            ref int index)
        {
            var hierarchy = new EntityHierarchy
            {
                Entity = entity,
                Alias = "[t" + index + "]",
                SubHierarchies = new List<EntityHierarchy>(),
                ParentHierarchy = parent
            };

            foreach (var property in entity.GetDefaultCreateProperties()
                .Where(x => x.IsForeignKey && x.TypeInfo.IsCollection)
                .Where(property =>
                    parent == null ||
                    parent.Entity != property.ForeignEntity))
            {
                index++;
                var subHierarchy =
                    GetEntityHierarchy(hierarchy, property.ForeignEntity, ref index);
                hierarchy.SubHierarchies.Add(subHierarchy);
            }

            return hierarchy;
        }

        // TODO: test it
        //SELECT [t0].*, [t1].*, [t2].*
        //FROM [Categories] AS [t0]
        //LEFT OUTER JOIN [Products] AS [t1] ON [t1].[CategoryID] = [t0].[CategoryID]
        //LEFT OUTER JOIN [Suppliers] AS [t2] ON [t2].[SupplierID] = [t1].[SupplierID]
        //WHERE [t0].CategoryID = @0 -- @0 = 9
        //ORDER BY [t0].[CategoryID], [t1].[ProductID], [t2].[SupplierID]
    }
}