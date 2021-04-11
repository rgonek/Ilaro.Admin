using System;
using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Models;
using Massive;

namespace Ilaro.Admin.Core.DataAccess
{
    public class RecordHierarchyFetcher : IRecordHierarchyFetcher
    {
        //private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordHierarchyFetcher));
        private readonly IIlaroAdmin _admin;

        public RecordHierarchyFetcher(IIlaroAdmin admin)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));

            _admin = admin;
        }

        public RecordHierarchy GetRecordHierarchy(
            EntityRecord entityRecord,
            IList<PropertyDeleteOption> deleteOptions = null)
        {
            //_log.InfoFormat(
            //    "Getting record hierarchy for entity record ({0}#{1})",
            //    entityRecord.Entity.Name,
            //    entityRecord.JoinedKeysWithNames);

            var hierarchy = GetEntityHierarchy(entityRecord.Entity, deleteOptions);
            var sql = GenerateHierarchySql(hierarchy);
            //_log.Debug($"Sql hierarchy: \r\n {sql}");
            var model = new DynamicModel(_admin.ConnectionStringName);
            var records = model.Query(sql, entityRecord.Keys.Select(x => x.Raw).ToArray()).ToList();

            var recordHierarchy = GetHierarchyRecords(records, hierarchy);

            return recordHierarchy;
        }

        public EntityHierarchy GetEntityHierarchy(
            Entity entity,
            IList<PropertyDeleteOption> deleteOptions = null)
        {
            var deleteOptionsDict = deleteOptions == null ?
                null :
                deleteOptions.ToDictionary(x => x.HierarchyName);
            var index = 0;
            return GetEntityHierarchy(null, entity, deleteOptionsDict, string.Empty, ref index);
        }

        private RecordHierarchy GetHierarchyRecords(
            IList<IDictionary<string, object>> records,
            EntityHierarchy hierarchy)
        {
            var baseRecord = records.FirstOrDefault();
            var prefix = hierarchy.Alias.Undecorate() + "_";
            var record = hierarchy.Entity.CreateRecord(baseRecord, prefix);

            var recordHierarchy = new RecordHierarchy
            {
                Entity = hierarchy.Entity,
                JoinedKeysValues = record.JoinedKeysValues,
                DisplayName = record.ToString(),
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
                    var entityRecord = hierarchy.Entity.CreateRecord(record, prefix);

                    if (entityRecord.Keys.Any(x => x.AsString.HasValue()))
                    {
                        var subRecord = new RecordHierarchy
                        {
                            Entity = hierarchy.Entity,
                            JoinedKeysValues = entityRecord.JoinedKeysValues,
                            DisplayName = entityRecord.ToString(),
                            SubRecordsHierarchies = new List<RecordHierarchy>()
                        };

                        if (parentHierarchy.SubRecordsHierarchies.FirstOrDefault(x => x.JoinedKeysValues == subRecord.JoinedKeysValues) == null &&
                            Matching(record, foreignKey, foreignKeyValue))
                        {
                            parentHierarchy.SubRecordsHierarchies.Add(subRecord);

                            GetHierarchyRecords(
                                subRecord,
                                records,
                                hierarchy.SubHierarchies,
                                hierarchy.Entity.Keys.Select(x => prefix + x.Column.Undecorate()).ToList(),
                                entityRecord.Keys.Select(x=>x.AsString).ToList());
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

            var columnsList = flatHierarchy.SelectMany(x => x.Entity.Properties.SkipOneToMany().Select(y => y.Column).Distinct()
                .Select(y => x.Alias + "." + y + " AS " + x.Alias.Undecorate() + "_" + y.Undecorate())).ToList();
            var commaSeparator = "," + Environment.NewLine + "         ";
            var columns = string.Join(commaSeparator, columnsList);

            var ordersList = flatHierarchy.SelectMany(x => x.Entity.Keys.Select(y => x.Alias + "." + y.Column)).ToList();
            var orders = string.Join(commaSeparator, ordersList);
            var joins = GetJoins(flatHierarchy);

            var constraintsList = new List<string>();
            var counter = 0;
            foreach (var key in hierarchy.Entity.Keys)
            {
                constraintsList.Add($"{hierarchy.Alias}.{key.Column} = @{counter++}");
            }
            var constraintSeparator = Environment.NewLine + "     AND ";
            var constraints = string.Join(constraintSeparator, constraintsList);

            var sql =
$@"  SELECT {columns}
    FROM {hierarchy.Entity.Table} AS {hierarchy.Alias}
    {joins}
   WHERE {constraints}
ORDER BY {orders};";

            return sql;
        }

        private string GetJoins(IList<EntityHierarchy> flatHierarchy)
        {
            var joins = new List<string>();
            foreach (var item in flatHierarchy.Where(x => x.ParentHierarchy != null))
            {
                var foreignTable = item.Entity.Table;
                var foreignAlias = item.Alias;
                string foreignKey, key;
                var alias = item.ParentHierarchy.Alias;

                var foreignProperty = item.Entity.Properties
                    .FirstOrDefault(x => x.ForeignEntity == item.ParentHierarchy.Entity);
                if (foreignProperty == null || foreignProperty.TypeInfo.IsCollection)
                {
                    foreignKey = item.Entity.Keys.FirstOrDefault().Column;
                    key = item.ParentHierarchy.Entity.Properties
                        .FirstOrDefault(x => x.ForeignEntity == item.Entity).Column;
                }
                else
                {
                    foreignKey = foreignProperty.Column;
                    key = item.ParentHierarchy.Entity.Keys.FirstOrDefault().Column;
                }

                var join = $@"LEFT OUTER JOIN {foreignTable} AS {foreignAlias} ON {foreignAlias}.{foreignKey} = {alias}.{key}";

                joins.Add(join);
            }

            var joinsSeparator = Environment.NewLine + "    ";

            return string.Join(joinsSeparator, joins);
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
            IDictionary<string, PropertyDeleteOption> deleteOptions,
            string hierarchyName,
            ref int index)
        {
            var hierarchy = new EntityHierarchy
            {
                Entity = entity,
                Alias = "[t" + index + "]",
                SubHierarchies = new List<EntityHierarchy>(),
                ParentHierarchy = parent
            };

            foreach (var property in entity.Properties
                .WhereOneToMany()
                .Where(property =>
                    parent == null ||
                    parent.Entity != property.ForeignEntity))
            {
                if (hierarchyName.HasValue())
                    hierarchyName += "-";
                hierarchyName += property.ForeignEntity.Name;
                var deleteOption = GetDeleteOption(hierarchyName, deleteOptions);
                if (deleteOption == CascadeOption.Delete ||
                    deleteOption == CascadeOption.AskUser)
                {
                    index++;
                    var subHierarchy =
                        GetEntityHierarchy(hierarchy, property.ForeignEntity, deleteOptions, hierarchyName, ref index);
                    hierarchy.SubHierarchies.Add(subHierarchy);
                }
            }

            return hierarchy;
        }

        private CascadeOption GetDeleteOption(
            string hierarchyName,
            IDictionary<string, PropertyDeleteOption> deleteOptions = null)
        {
            if (deleteOptions == null)
                return CascadeOption.Delete;

            if (deleteOptions.ContainsKey(hierarchyName))
                return deleteOptions[hierarchyName].DeleteOption;

            return CascadeOption.Delete;
        }
    }
}