using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;
using Ilaro.Admin.Services.Interfaces;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    [AuthorizeWrapper]
    public class EntitiesController : BaseController
    {
        private readonly IEntityService _entityService;

        public EntitiesController(Notificator notificator, IEntityService entityService)
            : base(notificator)
        {
            _entityService = entityService;
        }

        public virtual ActionResult Index(
            string entityName,
            int page = 1,
            [Bind(Prefix = "sq")]string searchQuery = "",
            [Bind(Prefix = "pp")]int perPage = 10,
            [Bind(Prefix = "o")]string order = "",
            [Bind(Prefix = "od")]string orderDirection = "")
        {
            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            var filters = _entityService.PrepareFilters(entity, Request);
            var pagedRecords = _entityService.GetRecords(
                entity,
                page,
                perPage,
                filters,
                searchQuery,
                order,
                orderDirection);
            if (pagedRecords.Records.IsNullOrEmpty() && page > 1)
            {
                return RedirectToAction(
                    "Index",
                    PrepareRouteValues(
                        entityName,
                        1,
                        perPage,
                        filters,
                        searchQuery,
                        order,
                        orderDirection));
            }

            var url = Url.Action(
                "Index",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    perPage,
                    filters,
                    searchQuery,
                    order, 
                    orderDirection))
                .Replace("-page-", "{0}");
            var model = new EntitiesIndexModel
            {
                Data = pagedRecords.Records,
                Columns = _entityService
                    .PrepareColumns(entity, order, orderDirection),
                Entity = entity,
                Pager =
                    new PagerInfo(url, perPage, page, pagedRecords.TotalItems),
                Filters = filters,
                SearchQuery = searchQuery,
                IsSearchActive = entity.SearchProperties.Any(),
                PerPage = perPage,
                Order = order,
                OrderDirection = orderDirection
            };

            return View(model);
        }

        public virtual ActionResult Changes(
            string entityName,
            int page = 1,
            [Bind(Prefix = "sq")]string searchQuery = "",
            [Bind(Prefix = "pp")]int perPage = 10,
            [Bind(Prefix = "o")]string order = "",
            [Bind(Prefix = "od")]string orderDirection = "")
        {
            var entityChangesFor = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            var changeEntity = AdminInitialise.ChangeEntity;
            var filters = _entityService.PrepareFilters(changeEntity, Request);
            var pagedRecords = _entityService.GetChangesRecords(
                entityChangesFor,
                page,
                perPage,
                filters,
                searchQuery,
                order,
                orderDirection);
            if (pagedRecords.Records.IsNullOrEmpty() && page > 1)
            {
                return RedirectToAction(
                    "Changes",
                    PrepareRouteValues(
                        entityName,
                        1,
                        perPage,
                        filters,
                        searchQuery,
                        order,
                        orderDirection));
            }

            var url = Url.Action(
                "Changes",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    perPage,
                    filters,
                    searchQuery,
                    order,
                    orderDirection))
                .Replace("-page-", "{0}");
            var model = new EntitiesChangesModel
            {
                Data = pagedRecords.Records,
                Columns = _entityService
                    .PrepareColumns(changeEntity, order, orderDirection),
                Entity = changeEntity,
                EntityChangesFor = entityChangesFor,
                Pager =
                    new PagerInfo(url, perPage, page, pagedRecords.TotalItems),
                Filters = filters,
                SearchQuery = searchQuery,
                IsSearchActive = changeEntity.SearchProperties.Any(),
                PerPage = perPage,
                Order = order,
                OrderDirection = orderDirection
            };

            return View(model);
        }

        protected virtual RouteValueDictionary PrepareRouteValues(
            string entityName,
            int page,
            int perPage,
            IList<IEntityFilter> filters,
            string searchQuery,
            string order,
            string orderDirection)
        {
            return PrepareRouteValues(
                entityName,
                page.ToString(),
                perPage,
                filters,
                searchQuery,
                order,
                orderDirection);
        }

        protected virtual RouteValueDictionary PrepareRouteValues(
            string entityName,
            string page,
            int perPage,
            IList<IEntityFilter> filters,
            string searchQuery,
            string order,
            string orderDirection)
        {
            var routeValues = new Dictionary<string, object>
            {
                { "entityName", entityName },
                { "page", page },
                { "pp", perPage }
            };

            if (!searchQuery.IsNullOrEmpty())
            {
                routeValues.Add("sq", searchQuery);
            }

            if (!order.IsNullOrEmpty() && !orderDirection.IsNullOrEmpty())
            {
                routeValues.Add("o", order);
                routeValues.Add("od", orderDirection);
            }

            foreach (var filter in filters.Where(x => !x.Value.IsNullOrEmpty()))
            {
                routeValues.Add(filter.Property.Name, filter.Value);
            }

            return new RouteValueDictionary(routeValues);
        }
    }
}
