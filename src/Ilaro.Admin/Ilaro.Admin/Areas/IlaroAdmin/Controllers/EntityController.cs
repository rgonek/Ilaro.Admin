using System;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Models;
using Resources;
using Ilaro.Admin.DataAnnotations;
using System.Linq;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    [AuthorizeWrapper]
    public class EntityController : Controller
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(EntityController));
        private readonly Notificator _notificator;
        private readonly IEntityService _entityService;
        private readonly IFetchingRecords _source;
        private readonly IFetchingRecordsHierarchy _hierarchySource;
        private readonly IIlaroAdmin _admin;

        public EntityController(
            IIlaroAdmin admin,
            Notificator notificator,
            IEntityService entityService,
            IFetchingRecords source,
            IFetchingRecordsHierarchy hierarchySource)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (entityService == null)
                throw new ArgumentNullException(nameof(entityService));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (hierarchySource == null)
                throw new ArgumentNullException(nameof(hierarchySource));

            _admin = admin;
            _notificator = notificator;
            _entityService = entityService;
            _source = source;
            _hierarchySource = hierarchySource;
        }

        public virtual ActionResult Create(string entityName)
        {
            var entity = _admin.GetEntity(entityName);
            if (entity == null)
            {
                return RedirectToAction("NotFound", new { entityName });
            }

            var entityRecord = EntityRecord.CreateEmpty(entity);
            var model = new EntityCreateModel
            {
                Entity = entity,
                PropertiesGroups = _entityService.PrepareGroups(entityRecord)
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(string entityName, FormCollection collection)
        {
            var entity = _admin.GetEntity(entityName);
            if (entity == null)
            {
                return RedirectToAction("NotFound", new { entityName });
            }

            try
            {
                var savedId = _entityService.Create(entity, collection, Request.Files);
                if (savedId != null)
                {
                    _notificator.Success(IlaroAdminResources.AddSuccess, entity.Verbose.Singular);

                    return SaveOrUpdateSucceed(entityName, savedId);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _notificator.Error(ex.Message);
            }

            var entityRecord = new EntityRecord(entity);
            entityRecord.Fill(collection, Request.Files);

            var model = new EntityCreateModel
            {
                Entity = entity,
                PropertiesGroups = _entityService.PrepareGroups(entityRecord)
            };

            return View(model);
        }

        public virtual ActionResult Edit(string entityName, string key)
        {
            var entity = _admin.GetEntity(entityName);
            if (entity == null)
            {
                return RedirectToAction("NotFound", new { entityName });
            }

            var entityRecord = _source.GetEntityRecord(entity, key);
            if (entityRecord == null)
            {
                return RedirectToAction("Index", "Entities", new { area = "IlaroAdmin", entityName });
            }

            var model = new EntityEditModel
            {
                Entity = entity,
                Record = entityRecord,
                PropertiesGroups = _entityService.PrepareGroups(entityRecord, getKey: false, key: key)
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Edit(string entityName, string key, FormCollection collection)
        {
            var entity = _admin.GetEntity(entityName);
            if (entity == null)
            {
                return RedirectToAction("NotFound", new { entityName });
            }

            try
            {
                var isSuccess = _entityService.Edit(entity, key, collection, Request.Files);
                if (isSuccess)
                {
                    _notificator.Success(IlaroAdminResources.EditSuccess, entity.Verbose.Singular);

                    return SaveOrUpdateSucceed(entityName, key);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _notificator.Error(ex.Message);
            }


            var entityRecord = new EntityRecord(entity);
            entityRecord.Fill(key, collection, Request.Files);

            var model = new EntityEditModel
            {
                Entity = entity,
                Record = entityRecord,
                PropertiesGroups = _entityService.PrepareGroups(entityRecord, getKey: false, key: key)
            };

            return View(model);
        }

        protected virtual ActionResult SaveOrUpdateSucceed(string entityName, string key)
        {
            if (Request["ContinueEdit"] != null)
                return RedirectToAction("Edit", new { entityName, key });
            if (Request["AddNext"] != null)
                return RedirectToAction("Create", new { entityName });
            return RedirectToAction("Index", "Entities", new { entityName });
        }

        public virtual ActionResult Delete(string entityName, string key)
        {
            var entity = _admin.GetEntity(entityName);
            if (entity == null)
            {
                return RedirectToAction("NotFound", new { entityName });
            }

            var entityRecord = _source.GetEntityRecord(entity, key);
            if (entityRecord == null)
            {
                return RedirectToAction("Index", "Entities", new { area = "IlaroAdmin", entityName });
            }

            var deleteOptions = DeleteOptionsHierarchyBuilder.GetHierarchy(entity);
            var model = new EntityDeleteModel(deleteOptions)
            {
                EntityRecord = entityRecord,
                RecordHierarchy = _hierarchySource.GetRecordHierarchy(entityRecord, deleteOptions)
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public virtual ActionResult Delete(EntityDeleteModel model)
        {
            var entity = _admin.GetEntity(model.EntityName);
            if (entity == null)
            {
                return RedirectToAction("NotFound", new { entityName = model.EntityName });
            }

            var deleteOptions = DeleteOptionsHierarchyBuilder.Merge(
                entity,
                model.PropertiesDeleteOptions);

            try
            {
                var isSuccess = _entityService.Delete(entity, model.Key, deleteOptions);
                if (isSuccess)
                {
                    _notificator.Success(IlaroAdminResources.DeleteSuccess, entity.Verbose.Singular);

                    return RedirectToAction("Index", "Entities", new { entityName = model.EntityName });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                _notificator.Error(ex.Message);
            }

            var entityRecord = _source.GetEntityRecord(entity, model.Key);

            model = new EntityDeleteModel(deleteOptions)
            {
                EntityRecord = entityRecord,
                RecordHierarchy = _hierarchySource.GetRecordHierarchy(entityRecord)
            };

            return View(model);
        }

        public virtual ActionResult NotFound(string entityName)
        {
            _log.ErrorFormat("Not found entity ({0}).", entityName);
            return Content("not found");
        }
    }
}
