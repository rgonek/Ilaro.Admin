using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    [AuthorizeWrapper]
    public class EntitiesController : Controller
    {
        private readonly Notificator _notificator;
        private readonly IFetchingRecords _entitiesSource;
        private readonly IConfiguration _configuration;

        public EntitiesController(
            Notificator notificator,
            IFetchingRecords entitiesSource,
            IConfiguration configuration)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (entitiesSource == null)
                throw new ArgumentException("entitiesSource");
            if (configuration == null)
                throw new ArgumentException("configuration");

            _notificator = notificator;
            _entitiesSource = entitiesSource;
            _configuration = configuration;
        }

        public virtual ActionResult Index(string entityName, TableInfo tableInfo)
        {
            var entity = Admin.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }
            var filters = PrepareFilters(entity);
            var pagedRecords = _entitiesSource.GetRecords(
                entity,
                filters,
                tableInfo.SearchQuery,
                tableInfo.Order,
                tableInfo.OrderDirection,
                false,
                tableInfo.Page,
                tableInfo.PerPage);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return RedirectToAction(
                    "Index",
                    PrepareRouteValues(
                        entityName,
                        "1",
                        filters,
                        tableInfo));
            }

            var url = Url.Action(
                "Index",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    filters,
                    tableInfo))
                .Replace("-page-", "{0}");

            var model = new EntitiesIndexModel
            {
                Data = pagedRecords.Records,
                Columns = entity.DisplayProperties
                    .Select(x => new Column(x, tableInfo.Order, tableInfo.OrderDirection)).ToList(),
                Entity = entity,
                Pager =
                    new PagerInfo(url, tableInfo.PerPage, tableInfo.Page, pagedRecords.TotalItems),
                Filters = filters,
                TableInfo = tableInfo,
                Configuration = _configuration
            };

            return View(model);
        }

        public virtual ActionResult Changes(string entityName, TableInfo tableInfo)
        {
            var entityChangesFor = Admin.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            if (entityChangesFor == null)
            {
                throw new NoNullAllowedException("entity is null");
            }
            var changeEntity = Admin.ChangeEntity;
            var filters = PrepareFilters(changeEntity);
            var filters2 = filters.ToList();
            filters2.Add(new ChangeEntityFilter(changeEntity["EntityName"], entityName));
            var pagedRecords = _entitiesSource.GetRecords(
                changeEntity,
                filters2,
                tableInfo.SearchQuery,
                tableInfo.Order,
                tableInfo.OrderDirection,
                false,
                tableInfo.Page,
                tableInfo.PerPage);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return RedirectToAction(
                    "Changes",
                    PrepareRouteValues(
                        entityName,
                        "1",
                        filters,
                        tableInfo));
            }

            var url = Url.Action(
                "Changes",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    filters,
                    tableInfo))
                .Replace("-page-", "{0}");
            var model = new EntitiesChangesModel
            {
                Data = pagedRecords.Records,
                Columns = changeEntity.DisplayProperties
                    .Select(x => new Column(x, tableInfo.Order, tableInfo.OrderDirection)).ToList(),
                Entity = changeEntity,
                EntityChangesFor = entityChangesFor,
                Pager =
                    new PagerInfo(url, tableInfo.PerPage, tableInfo.Page, pagedRecords.TotalItems),
                Filters = filters,
                TableInfo = tableInfo,
                Configuration = _configuration
            };

            return View(model);
        }

        protected virtual RouteValueDictionary PrepareRouteValues(
            string entityName,
            string page,
            IEnumerable<IEntityFilter> filters,
            TableInfo tableInfo)
        {
            var routeValues = new Dictionary<string, object>
            {
                { "entityName", entityName },
                { _configuration.PageRequestName, page },
                { _configuration.PerPageRequestName, tableInfo.PerPage }
            };

            if (!tableInfo.SearchQuery.IsNullOrEmpty())
            {
                routeValues.Add(_configuration.SearchQueryRequestName, tableInfo.SearchQuery);
            }

            if (!tableInfo.Order.IsNullOrEmpty() && !tableInfo.OrderDirection.IsNullOrEmpty())
            {
                routeValues.Add(_configuration.OrderRequestName, tableInfo.Order);
                routeValues.Add(_configuration.OrderDirectionRequestName, tableInfo.OrderDirection);
            }

            foreach (var filter in filters.Where(x => !x.Value.IsNullOrEmpty()))
            {
                routeValues.Add(filter.Property.Name, filter.Value);
            }

            return new RouteValueDictionary(routeValues);
        }

        public IList<BaseFilter> PrepareFilters(Entity entity)
        {
            var filters = new List<BaseFilter>();

            foreach (var property in entity.Properties.Where(x => x.TypeInfo.DataType == DataType.Bool))
            {
                var value = Request[property.Name];

                filters.Add(new BoolEntityFilter(property, value));
            }

            foreach (var property in entity.Properties.Where(x => x.TypeInfo.DataType == DataType.Enum))
            {
                var value = Request[property.Name];

                filters.Add(new EnumEntityFilter(property, value));
            }

            foreach (var property in entity.Properties.Where(x => x.TypeInfo.DataType == DataType.DateTime))
            {
                var value = Request[property.Name];

                filters.Add(new DateTimeEntityFilter(SystemClock.Instance, property, value));
            }

            return filters;
        }
    }
}
