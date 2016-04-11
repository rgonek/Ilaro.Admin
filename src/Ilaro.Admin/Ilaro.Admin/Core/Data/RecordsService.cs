using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsService : IRecordsService
    {
        private readonly IIlaroAdmin _admin;
        private readonly IFetchingRecords _entitiesSource;
        private readonly IFilterFactory _filterFactory;

        public RecordsService(
            IIlaroAdmin admin,
            IFetchingRecords entitiesSource,
            IFilterFactory filterFactory)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (entitiesSource == null)
                throw new ArgumentNullException(nameof(entitiesSource));
            if (filterFactory == null)
                throw new ArgumentNullException(nameof(filterFactory));

            _admin = admin;
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

        public PagedRecords GetChanges(
            Entity entityChangesFor,
            string key,
            NameValueCollection request,
            TableInfo tableInfo)
        {
            var changeEntity = _admin.ChangeEntity;
            return GetRecords(changeEntity, request, tableInfo, filters =>
            {
                if (key.IsNullOrWhiteSpace() == false)
                {
                    filters.Add(new ForeignEntityFilter(changeEntity["EntityKey"], key));
                }
                if (entityChangesFor != null)
                {
                    filters.Add(new ChangeEntityFilter(changeEntity["EntityName"], entityChangesFor.Name));
                }
            });
        }

        public IList<ChangeRow> GetLastChanges(int quantity)
        {
            var changeEntity = _admin.ChangeEntity;
            if (changeEntity == null)
            {
                return new List<ChangeRow>();
            }

            var tableInfo = new TableInfo
            {
                Page = 1,
                PerPage = quantity,
                Order = nameof(IEntityChange.ChangedOn),
                OrderDirection = "desc"
            };
            var pagedRecords = GetRecords(changeEntity, null, tableInfo);

            return pagedRecords.Records.Select(x => new ChangeRow(x)).ToList();
        }

        private PagedRecords GetRecords(
            Entity entity,
            NameValueCollection request,
            TableInfo tableInfo,
            Action<IList<BaseFilter>> filtersMutator)
        {
            var filterRecord = create_filter_record(entity, request);
            var filters = _filterFactory.BuildFilters(filterRecord).ToList();
            if (filtersMutator != null)
            {
                filtersMutator(filters);
            }
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

        private static EntityRecord create_filter_record(
            Entity entity,
            NameValueCollection request)
        {
            var filterRecord = new EntityRecord(entity);
            if (request != null)
                filterRecord.Fill(request);

            return filterRecord;
        }
    }
}