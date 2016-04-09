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
        private readonly IIlaroAdmin _admin;
        private readonly Notificator _notificator;
        private readonly IConfiguration _configuration;
        private readonly IRecordsService _recordsService;

        public EntitiesController(
            IIlaroAdmin admin,
            Notificator notificator,
            IRecordsService recordsService,
            IConfiguration configuration)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (recordsService == null)
                throw new ArgumentException(nameof(recordsService));
            if (configuration == null)
                throw new ArgumentException(nameof(configuration));

            _admin = admin;
            _notificator = notificator;
            _recordsService = recordsService;
            _configuration = configuration;
        }

        public virtual ActionResult Index(string entityName, TableInfo tableInfo)
        {
            var entity = _admin.GetEntity(entityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }
            var pagedRecords = _recordsService.GetRecords(entity, Request.Form, tableInfo);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return RedirectToAction(
                    "Index",
                    PrepareRouteValues(
                        entityName,
                        "1",
                        pagedRecords.Filters,
                        tableInfo));
            }

            var url = Url.Action(
                "Index",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    pagedRecords.Filters,
                    tableInfo))
                .Replace("-page-", "{0}");

            var model = new EntitiesIndexModel(entity, pagedRecords, tableInfo, url)
            {
                Configuration = _configuration,
                ChangeEnabled = _admin.ChangeEntity != null
            };

            return View(model);
        }

        public virtual ActionResult Changes(string entityName, string key, TableInfo tableInfo)
        {
            var entityChangesFor = _admin.GetEntity(entityName);
            var pagedRecords = _recordsService
                .GetChanges(entityChangesFor, key, Request.Form, tableInfo);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return RedirectToAction(
                    "Changes",
                    PrepareRouteValues(
                        entityName,
                        "1",
                        pagedRecords.Filters,
                        tableInfo));
            }

            var url = Url.Action(
                "Changes",
                PrepareRouteValues(
                    entityName,
                    "-page-",
                    pagedRecords.Filters,
                    tableInfo))
                .Replace("-page-", "{0}");

            var model = new EntitiesChangesModel(
                _admin.ChangeEntity,
                pagedRecords,
                tableInfo,
                url)
            {
                EntityChangesFor = entityChangesFor,
                Key = key,
                Configuration = _configuration,
                ChangeEnabled = _admin.ChangeEntity != null
            };

            return View(model);
        }

        protected virtual RouteValueDictionary PrepareRouteValues(
            string entityName,
            string page,
            IEnumerable<BaseFilter> filters,
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

            foreach (var filter in filters.Where(x => x.DisplayInUI && !x.Value.IsNullOrEmpty()))
            {
                routeValues.Add(filter.Property.Name, filter.Value);
            }

            return new RouteValueDictionary(routeValues);
        }
    }
}
