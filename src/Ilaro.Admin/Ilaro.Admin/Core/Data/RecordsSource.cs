using System;
using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;
using Massive;
using Resources;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsSource : IFetchingRecords
    {
        private readonly Notificator _notificator;

        public RecordsSource(Notificator notificator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");

            _notificator = notificator;
        }

        public Entity GetEntityWithData(string entityName, string key)
        {
            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);

            if (entity == null)
                return null;

            var item = GetRecord(entity, entity.Key.Value.ToObject(key));
            if (item == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return null;
            }

            var propertiesDict = item as IDictionary<string, object>;

            foreach (var property in entity.CreateProperties(false))
            {
                property.Value.Raw = 
                    propertiesDict.ContainsKey(property.ColumnName) ?
                    propertiesDict[property.ColumnName] :
                    null;
            }

            return entity;
        }

        public object GetRecord(Entity entity, object key)
        {
            var table = new DynamicModel(
                AdminInitialise.ConnectionString, 
                tableName: entity.TableName, 
                primaryKeyField: entity.Key.ColumnName);

            var result = table.Single(key);

            return result;
        }

        public PagedRecords GetRecords(
            Entity entity,
            int page,
            int take,
            IList<IEntityFilter> filters,
            string searchQuery,
            string order,
            string orderDirection)
        {
            var search = new EntitySearch
            {
                Query = searchQuery,
                Properties = entity.SearchProperties
            };
            order = order.IsNullOrEmpty() ? entity.Key.ColumnName : order;
            orderDirection = orderDirection.IsNullOrEmpty() ?
                "ASC" :
                orderDirection.ToUpper();
            var orderBy = order + " " + orderDirection;
            var columns = string.Join(",", entity.GetColumns());
            var where = ConvertFiltersToSql(filters, search);

            var table = new DynamicModel(
                AdminInitialise.ConnectionString,
                entity.TableName,
                entity.Key.ColumnName);

            var result = table.Paged(
                columns: columns,
                where: where,
                orderBy: orderBy,
                currentPage: page,
                pageSize: take);

            var data = new List<DataRow>();
            foreach (var item in result.Items)
            {
                data.Add(new DataRow(item, entity));
            }

            return new PagedRecords
            {
                TotalItems = result.TotalRecords,
                TotalPages = result.TotalPages,
                Records = data
            };
        }

        public IList<DataRow> GetRecords(
            Entity entity,
            IList<IEntityFilter> filters = null,
            string searchQuery = null,
            string order = null,
            string orderDirection = null,
            bool determineDisplayValue = false)
        {
            var search = new EntitySearch
            {
                Query = searchQuery,
                Properties = entity.SearchProperties
            };
            order = order.IsNullOrEmpty() ? entity.Key.ColumnName : order;
            orderDirection = orderDirection.IsNullOrEmpty() ?
                "ASC" :
                orderDirection.ToUpper();
            var orderBy = order + " " + orderDirection;
            var columns = string.Join(",",
                entity.Properties
                    .Where(x =>
                        !x.IsForeignKey ||
                        (!x.TypeInfo.IsCollection && x.IsForeignKey))
                    .Select(x => x.ColumnName)
                    .Distinct());
            var where = ConvertFiltersToSql(filters, search);

            var table = new DynamicModel(
                AdminInitialise.ConnectionString,
                entity.TableName,
                entity.Key.ColumnName);

            var result = table.All(columns: columns, where: where, orderBy: orderBy);

            var data = result
                .Select(item => new DataRow(item, entity))
                .ToList();

            if (determineDisplayValue)
            {
                foreach (var row in data)
                {
                    row.DisplayName = entity.ToString(row);
                }
            }

            return data;
        }

        public PagedRecords GetChangesRecords(
            Entity entityChangesFor,
            int page,
            int take,
            IList<IEntityFilter> filters,
            string searchQuery,
            string order,
            string orderDirection)
        {
            var changeEntity = AdminInitialise.ChangeEntity;

            var search = new EntitySearch
            {
                Query = searchQuery,
                Properties = changeEntity.SearchProperties
            };
            order = order.IsNullOrEmpty() ? changeEntity.Key.ColumnName : order;
            orderDirection = orderDirection.IsNullOrEmpty() ?
                "ASC" :
                orderDirection.ToUpper();
            var orderBy = order + " " + orderDirection;
            var columns = string.Join(",", changeEntity.GetColumns());
            var where = ConvertFiltersToSql(filters, search);
            if (where.IsNullOrEmpty())
            {
                where += " WHERE EntityName = '" + entityChangesFor.Name + "'";
            }
            else
            {
                where += " AND EntityName = '" + entityChangesFor.Name + "'";
            }
            var table = new DynamicModel(
                AdminInitialise.ConnectionString,
                changeEntity.TableName,
                changeEntity.Key.Name);

            var result = table.Paged(
                columns: columns,
                where: where,
                orderBy: orderBy,
                currentPage: page,
                pageSize: take);

            var data = new List<DataRow>();
            foreach (var item in result.Items)
            {
                data.Add(new DataRow(item, changeEntity));
            }

            return new PagedRecords
            {
                TotalItems = result.TotalRecords,
                TotalPages = result.TotalPages,
                Records = data
            };
        }

        private static string ConvertFiltersToSql(
            IList<IEntityFilter> filters,
            EntitySearch search,
            string alias = "")
        {
            if (filters == null)
            {
                filters = new List<IEntityFilter>();
            }

            var activeFilters = filters
                .Where(x => !x.Value.IsNullOrEmpty())
                .ToList();
            if (!activeFilters.Any() && !search.IsActive)
            {
                return null;
            }

            if (!alias.IsNullOrEmpty())
            {
                alias += ".";
            }

            var conditions = String.Empty;
            foreach (var filter in activeFilters)
            {
                var condition = filter.GetSqlCondition(alias);

                if (!condition.IsNullOrEmpty())
                {
                    conditions += " {0} AND".Fill(filter.GetSqlCondition(alias));
                }
            }

            if (search.IsActive)
            {
                var searchCondition = String.Empty;
                foreach (var property in search.Properties)
                {
                    var query = search.Query.TrimStart('>', '<');
                    decimal temp;
                    if (property.TypeInfo.IsString)
                    {
                        searchCondition += " {0}[{1}] LIKE '%{2}%' OR"
                            .Fill(alias, property.Name, search.Query);
                    }
                    else if (decimal.TryParse(query, out temp))
                    {
                        var sign = "=";
                        if (search.Query.StartsWith(">"))
                        {
                            sign = ">=";
                        }
                        else if (search.Query.StartsWith("<"))
                        {
                            sign = "<=";
                        }

                        searchCondition += " {0}[{1}] {3} {2} OR"
                            .Fill(alias, property.Name, query.Replace(",", "."), sign);
                    }
                }

                if (!searchCondition.IsNullOrEmpty())
                {
                    conditions +=
                        " (" +
                        searchCondition
                            .TrimStart(' ')
                            .TrimEnd("OR".ToCharArray()) +
                        ")";
                }
            }

            if (conditions.IsNullOrEmpty())
            {
                return null;
            }

            if (conditions.IsNullOrEmpty())
            {
                return String.Empty;
            }
            return " WHERE" + conditions.TrimEnd("AND".ToCharArray());
        }
    }
}