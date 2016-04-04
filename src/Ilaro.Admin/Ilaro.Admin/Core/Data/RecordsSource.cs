using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;
using Massive;
using Resources;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsSource : IFetchingRecords
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordsSource));
        private readonly Notificator _notificator;
        private readonly IIlaroAdmin _admin;

        public RecordsSource(IIlaroAdmin admin, Notificator notificator)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));

            _admin = admin;
            _notificator = notificator;
        }

        public EntityRecord GetEntityRecord(Entity entity, string key)
        {
            var keys = key.Split(Const.KeyColSeparator).Select(x => x.Trim()).ToArray();

            return GetEntityRecord(entity, keys);
        }

        public EntityRecord GetEntityRecord(Entity entity, params string[] key)
        {
            var keys = new object[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                keys[i] = new PropertyValue(entity.Key[i]).ToObject(key[i]);
            }
            var item = GetRecord(entity, keys);
            if (item == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return null;
            }

            var entityRecord = new EntityRecord(entity);
            entityRecord.Fill(item);

            return entityRecord;
        }

        public IDictionary<string, object> GetRecord(Entity entity, string key)
        {
            var keys = key
                .Split(Const.KeyColSeparator)
                .Select(x => x.Trim())
                .ToArray();

            return GetRecord(entity, keys);
        }

        public IDictionary<string, object> GetRecord(Entity entity, params object[] key)
        {
            var table = new DynamicModel(
                _admin.ConnectionStringName,
                tableName: entity.Table,
                primaryKeyField: entity.JoinedKey);

            var result = table.Single(key);

            return result;
        }

        public PagedRecords GetRecords(
            Entity entity,
            IList<BaseFilter> filters = null,
            string searchQuery = null,
            string order = null,
            string orderDirection = null,
            bool determineDisplayValue = false,
            int? page = null,
            int? take = null)
        {
            var search = new EntitySearch
            {
                Query = searchQuery,
                Properties = entity.SearchProperties
            };
            order = order.IsNullOrEmpty() ? entity.Key.FirstOrDefault().Column : order;
            orderDirection = orderDirection.IsNullOrEmpty() ?
                "ASC" :
                orderDirection.ToUpper();
            var orderBy = order + " " + orderDirection;
            var columns = string.Join(",",
                entity.Properties
                    .Where(x =>
                        !x.IsForeignKey ||
                        (!x.TypeInfo.IsCollection && x.IsForeignKey))
                    .Select(x => x.Column)
                    .Distinct());
            List<object> args;
            var where = ConvertFiltersToSql(filters, search, out args);

            var table = new DynamicModel(
                _admin.ConnectionStringName,
                entity.Table,
                entity.JoinedKey);

            if (page.HasValue && take.HasValue)
            {
                var result = table.Paged(
                    columns: columns,
                    where: where,
                    orderBy: orderBy,
                    currentPage: page.Value,
                    pageSize: take.Value,
                    args: args.ToArray());

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
            else
            {
                var result = table.All(
                    columns: columns,
                    where: where,
                    orderBy: orderBy,
                    args: args.ToArray());

                var data = result
                    .Select(item => new DataRow(item, entity))
                    .ToList();

                if (determineDisplayValue)
                {
                    foreach (var row in data)
                    {
                        row.DisplayName = row.ToString(entity);
                    }
                }

                return new PagedRecords
                {
                    Records = data
                };
            }
        }

        private string ConvertFiltersToSql(
            IList<BaseFilter> filters,
            EntitySearch search,
            out List<object> args,
            string alias = "")
        {
            args = new List<object>();
            filters = filters ?? new List<BaseFilter>();

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

            var sbConditions = new StringBuilder();
            foreach (var filter in activeFilters)
            {
                var condition = filter.GetSqlCondition(alias, ref args);

                if (!condition.IsNullOrEmpty())
                {
                    sbConditions.AppendFormat(" {0} AND", condition);
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
                        searchCondition += " {0}{1} LIKE @{2} OR"
                            .Fill(alias, property.Column, args.Count);
                        args.Add("%" + search.Query + "%");
                    }
                    else if (decimal.TryParse(query.Replace(",", "."), NumberStyles.Any, CultureInfo.CurrentCulture, out temp))
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

                        searchCondition += " {0}{1} {2} @{3} OR"
                            .Fill(alias, property.Column, sign, args.Count);
                        args.Add(temp);
                    }
                }

                if (!searchCondition.IsNullOrEmpty())
                {
                    searchCondition = searchCondition
                        .TrimStart(' ')
                        .TrimEnd("OR".ToCharArray());
                    sbConditions.AppendFormat(" ({0})", searchCondition);
                }
            }

            var conditions = sbConditions.ToString();
            if (conditions.IsNullOrEmpty())
            {
                return null;
            }

            return " WHERE" + conditions.TrimEnd("AND".ToCharArray());
        }
    }
}