using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Filters;
using Ilaro.Admin.Core.Models;
using Massive;
using SqlKata;
using Ilaro.Admin.Core.DataAccess.Extensions;
using SqlKata.Execution;
using Dawn;

namespace Ilaro.Admin.Core.DataAccess
{
    public class RecordFetcher : IRecordFetcher
    {
        private readonly QueryFactory _db;

        public RecordFetcher(QueryFactory db)
        {
            Guard.Argument(db, nameof(db)).NotNull();

            _db = db;
        }

        public EntityRecord GetEntityRecord(Entity entity, string value)
        {
            var values = value
                .Split(Id.ColumnSeparator)
                .Select(x => x.Trim())
                .ToArray();

            return GetEntityRecord(entity, values);
        }

        public EntityRecord GetEntityRecord(Entity entity, params object[] values)
            => GetEntityRecord(entity, entity.Id.Fill(values));

        public EntityRecord GetEntityRecord(Entity entity, IdValue idValue)
        {
            var item = GetRecord(entity, idValue);
            if (item == null)
            {
                return null;
            }

            return entity.CreateRecord(item);
        }

        public IDictionary<string, object> GetRecord(Entity entity, string value)
        {
            var values = value
                .Split(Id.ColumnSeparator)
                .Select(x => x.Trim())
                .ToArray();

            return GetRecord(entity, values);
        }

        public IDictionary<string, object> GetRecord(Entity entity, params object[] values)
            => GetRecord(entity, entity.Id.Fill(values));

        public IDictionary<string, object> GetRecord(Entity entity, IdValue idValue)
        {
            var query = _db.Query(entity.Table)
                .Select(entity.SelectableColumns.ToArray());

            foreach (var keyValue in idValue)
            {
                query.Where(keyValue.Property.Column, keyValue.AsObject);
            }

            return query.First();
        }

        public PagedRecords GetRecords(
            Entity entity,
            IList<BaseFilter> filters = null,
            string searchQuery = null,
            string order = null,
            string orderDirection = null,
            bool determineDisplayValue = false,
            int? page = null,
            int? take = null,
            bool loadForeignKeys = false)
        {
            var orderedColumn = order.IsNullOrEmpty() ? entity.Id.Keys.First().Column : order;
            var query = _db.Query(entity.Table)
                .Select(entity.SelectableColumns.ToArray())
                .OrderBy(orderedColumn, OrderDirection.Asc);

            AddFilters(query, filters, new EntitySearch(searchQuery, entity.SearchProperties));

            if (page.HasValue && take.HasValue)
            {
                query.ForPage(page.Value, take.Value);
            }

            var records = query.Get()
                .Select(item => entity.CreateRecord((IDictionary<string, object>)item))
                .ToList();

            return new PagedRecords
            {
                //TotalItems = result.TotalRecords,
                //TotalPages = result.TotalPages,
                Records = records
            };
            //var joins = "";
            //if (loadForeignKeys)
            //{
            //    foreach (var foreignKey in entity.ForeignKeys.WhereOneToMany())
            //    {
            //        var joinTable = foreignKey.ForeignEntity.Table;
            //        var joinProperty = foreignKey.ForeignEntity.Keys.FirstOrDefault(x => x.ForeignEntity == entity);

            //        var keyProperty = foreignKey.TypeInfo.IsCollection ?
            //            entity.Keys.FirstOrDefault() :
            //            foreignKey;

            //        joins += Environment.NewLine +
            //            $"left join {joinTable} on {joinTable}.{joinProperty.Column} = {entity.Table}.{keyProperty.Column}";

            //        var propertyToGet = foreignKey.ForeignEntity.Keys.FirstOrDefault(x => x.ForeignEntity != entity) ??
            //                            joinProperty;

            //        //columns += $",{joinTable}.{propertyToGet.Column} as {foreignKey.Column}";
            //    }
            //}
        }

        private void AddFilters(
            Query query,
            IList<BaseFilter> filters,
            EntitySearch search,
            string alias = "")
        {
            filters = filters ?? new List<BaseFilter>();

            var activeFilters = filters
                .Where(x => !x.Value.IsNullOrEmpty())
                .ToList();
            if (!activeFilters.Any() && !search.IsActive)
            {
                return;
            }

            if (!alias.IsNullOrEmpty())
            {
                alias += ".";
            }

            var sbConditions = new StringBuilder();
            foreach (var filter in activeFilters)
            {
                filter.AddCondition(query);
            }

            if (search.IsActive)
            {
                foreach (var property in search.Properties)
                {
                    var searchQuery = search.Query.TrimStart('>', '<');
                    if (property.TypeInfo.IsString)
                    {
                        query.OrWhereLike(property.Column, search.Query);
                    }
                    else if (decimal.TryParse(searchQuery.Replace(",", "."), NumberStyles.Any, CultureInfo.CurrentCulture, out var number))
                    {
                        var sign = search.Query[0] switch
                        {
                            '>' => ">=",
                            '<' => "<=",
                            _ => "="
                        };

                        query.OrWhere(property.Column, sign, number);
                    }
                }
            }
        }
    }
}