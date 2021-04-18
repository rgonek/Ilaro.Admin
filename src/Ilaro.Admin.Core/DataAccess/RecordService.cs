using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Filters;
using Ilaro.Admin.Core.Models;

namespace Ilaro.Admin.Core.DataAccess
{
    public class RecordService : IRecordService
    {
        private readonly IRecordFetcher _entitiesSource;
        private readonly IFilterFactory _filterFactory;

        public RecordService(
            IRecordFetcher entitiesSource,
            IFilterFactory filterFactory)
        {
            if (entitiesSource == null)
                throw new ArgumentNullException(nameof(entitiesSource));
            if (filterFactory == null)
                throw new ArgumentNullException(nameof(filterFactory));

            _entitiesSource = entitiesSource;
            _filterFactory = filterFactory;
        }

        public PagedRecords GetRecords(
            Entity entity,
            NameValueCollection request,
            TableInfo tableInfo)
        {
            return GetRecords(entity, request, tableInfo, null);
        }

        private PagedRecords GetRecords(
            Entity entity,
            NameValueCollection request,
            TableInfo tableInfo,
            Action<IList<BaseFilter>> filtersMutator)
        {
            AddDefaultOrder(entity, tableInfo);

            var filters = BuildFilters(entity, request, filtersMutator).ToList();

            var pagedRecords = _entitiesSource.GetRecords(
                entity,
                filters,
                tableInfo.SearchQuery,
                tableInfo.Order,
                tableInfo.OrderDirection,
                false,
                tableInfo.Page,
                tableInfo.PerPage);
            pagedRecords.Filters = filters;

            return pagedRecords;
        }

        private IEnumerable<BaseFilter> BuildFilters(
            Entity entity,
            NameValueCollection request,
            Action<IList<BaseFilter>> filtersMutator)
        {
            var filterRecord = create_filter_record(entity, request);
            var filters = _filterFactory.BuildFilters(filterRecord).ToList();
            filtersMutator?.Invoke(filters);

            return filters;
        }

        private void AddDefaultOrder(Entity entity, TableInfo tableInfo)
        {
            if (tableInfo.Order.HasValue())
                return;

            var defaultOrderProperty = entity.Properties
                .FirstOrDefault(x => x.DefaultOrder.HasValue);
            if (defaultOrderProperty != null)
            {
                tableInfo.Order = defaultOrderProperty.Name;
                tableInfo.OrderDirection =
                    defaultOrderProperty.DefaultOrder.Value.ToString().ToLower();
            }
        }

        private static EntityRecord create_filter_record(
            Entity entity,
            NameValueCollection request)
        {
            return request == null ?
                entity.CreateEmptyRecord() :
                entity.CreateRecord(request, valueMutator: x => (string)x == Const.EmptyFilterValue ? "" : x);
        }
    }
}