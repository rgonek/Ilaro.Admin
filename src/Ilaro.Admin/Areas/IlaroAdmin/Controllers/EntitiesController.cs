using System;
using System.Data;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.DataAnnotations;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Models;
using Ilaro.Admin.Extensions;

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
            var pagedRecords = _recordsService.GetRecords(entity, Request.QueryString, tableInfo);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return Redirect(Url.PageUrl(1));
            }

            var model = new EntitiesIndexModel(entity, pagedRecords, tableInfo)
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
                .GetChanges(entityChangesFor, key, Request.QueryString, tableInfo);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {
                return Redirect(Url.PageUrl(1));
            }

            var model = new EntitiesChangesModel(
                _admin.ChangeEntity,
                pagedRecords,
                tableInfo)
            {
                EntityChangesFor = entityChangesFor,
                Key = key,
                Configuration = _configuration,
                ChangeEnabled = _admin.ChangeEntity != null
            };

            return View(model);
        }
    }
}
