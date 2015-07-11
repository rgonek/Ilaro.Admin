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
        public RecordHierarchy GetRecordHierarchy(Entity entity)
        {
            var index = 0;
            var hierarchy = GetEntityHierarchy(null, entity, ref index);
            var sql = GenerateHierarchySql(hierarchy);
            var model = new DynamicModel(Admin.ConnectionStringName);
            var records = model.Query(sql, entity.Key.Value.Raw).ToList();

            var recordHierarchy = GetHierarchyRecords(records, hierarchy);

            return recordHierarchy;
        }

        private RecordHierarchy GetHierarchyRecords(
            IList<dynamic> records,
            EntityHierarchy hierarchy)
        {
            var baseRecord = records.FirstOrDefault();
            var prefix = hierarchy.Alias.UnDecorate() + "_";
            var rowData = new DataRow(baseRecord, hierarchy.Entity, prefix);

            var recordHierarchy = new RecordHierarchy
            {
                Entity = hierarchy.Entity,
                KeyValue = rowData.KeyValue,
                DisplayName = hierarchy.Entity.ToString(rowData),
                SubRecordsHierarchies = new List<RecordHierarchy>()
            };

            GetHierarchyRecords(recordHierarchy, records, hierarchy.SubHierarchies);

            return recordHierarchy;
        }

        private void GetHierarchyRecords(
            RecordHierarchy parentHierarchy, 
            IList<dynamic> records, 
            IEnumerable<EntityHierarchy> subHierarchies, 
            string foreignKey = null, 
            string foreignKeyValue = null)
        {
            foreach (var hierarchy in subHierarchies)
            {
                var prefix = hierarchy.Alias.Trim("[]".ToCharArray()) + "_";

                foreach (var record in records)
                {
                    var recordDict = (IDictionary<string, object>)record;
                    var rowData = new DataRow(recordDict, hierarchy.Entity, prefix);

                    if (!rowData.KeyValue.IsNullOrEmpty())
                    {
                        var subRecord = new RecordHierarchy
                        {
                            Entity = hierarchy.Entity,
                            KeyValue = rowData.KeyValue,
                            DisplayName = hierarchy.Entity.ToString(rowData),
                            SubRecordsHierarchies = new List<RecordHierarchy>()
                        };

                        if (parentHierarchy.SubRecordsHierarchies.FirstOrDefault(x => x.KeyValue == subRecord.KeyValue) == null &&
                            (foreignKey.IsNullOrEmpty() || recordDict[foreignKey].ToStringSafe() == foreignKeyValue))
                        {
                            parentHierarchy.SubRecordsHierarchies.Add(subRecord);

                            GetHierarchyRecords(
                                subRecord, 
                                records, 
                                hierarchy.SubHierarchies, 
                                prefix + hierarchy.Entity.Key.ColumnName, 
                                rowData.KeyValue);
                        }
                    }
                }
            }
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

            var columns = flatHierarchy.SelectMany(x => x.Entity.GetColumns()
                .Select(y => x.Alias + "." + y + " AS " + x.Alias.UnDecorate() + "_" + y.UnDecorate())).ToList();
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
                    foreignKey = item.Entity.Key.ColumnName;
                    baseTablePrimaryKey = item.ParentHierarchy.Entity.Properties
                        .FirstOrDefault(x => x.ForeignEntity == item.Entity).ColumnName;
                }
                else
                {
                    foreignKey = foreignProperty.ColumnName;
                    baseTablePrimaryKey = item.ParentHierarchy.Entity.Key.ColumnName;
                }
                joins.Add(
                    string.Format(joinFormat, 
                        foreignTable, 
                        foreignAlias, 
                        foreignKey, 
                        baseTableAlias, 
                        baseTablePrimaryKey));
            }
            var orders = flatHierarchy.Select(x => x.Alias + "." + x.Entity.Key.ColumnName).ToList();

            var where = hierarchy.Alias + "." + hierarchy.Entity.Key.ColumnName + " = @0";

            var sql = string.Format(selectFormat,
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

            foreach (var property in entity.CreateProperties()
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
        //			SELECT [t0].*, [t1].*, [t2].*
        //FROM [Categories] AS [t0]
        //LEFT OUTER JOIN [Products] AS [t1] ON [t1].[CategoryID] = [t0].[CategoryID]
        //LEFT OUTER JOIN [Suppliers] AS [t2] ON [t2].[SupplierID] = [t1].[SupplierID]
        //WHERE [t0].CategoryID = @0 -- @0 = 9
        //ORDER BY [t0].[CategoryID], [t1].[ProductID], [t2].[SupplierID]
    }
}