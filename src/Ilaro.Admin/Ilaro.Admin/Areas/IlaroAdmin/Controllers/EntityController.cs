using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Models;
using Ilaro.Admin.Services;
using Resources;
using Ilaro.Admin.Validation;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class EntityController : Controller
    {
        private readonly Notificator _notificator;
        private readonly IEntityService _entityService;
        private readonly IFetchingRecords _source;
        private readonly IFetchingRecordsHierarchy _hierarchySource;
        private readonly IValidateEntity _validator;

        public EntityController(
            Notificator notificator,
            IEntityService entityService,
            IValidateEntity validator,
            IFetchingRecords source,
            IFetchingRecordsHierarchy hierarchySource)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (entityService == null)
                throw new ArgumentNullException("entityService");
            if (validator == null)
                throw new ArgumentNullException("validator");
            if (source == null)
                throw new ArgumentNullException("source");
            if (hierarchySource == null)
                throw new ArgumentNullException("hierarchySource");

            _notificator = notificator;
            _entityService = entityService;
            _validator = validator;
            _source = source;
            _hierarchySource = hierarchySource;
        }

        public virtual ActionResult Create(string entityName)
        {
            ViewBag.IsAjaxRequest = HttpContext.Request.IsAjaxRequest();

            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }

            entity.ClearPropertiesValues();

            var model = new EntityCreateModel
            {
                Entity = entity,
                PropertiesGroups = _entityService.PrepareGroups(entity)
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(string entityName, FormCollection collection)
        {
            ViewBag.IsAjaxRequest = HttpContext.Request.IsAjaxRequest();

            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }

            entity.Fill(collection, Request.Files);
            if (_validator.Validate(entity))
            {
                try
                {
                    var savedId = _entityService.Create(entity);
                    if (savedId != null)
                    {
                        _notificator.Success(IlaroAdminResources.AddSuccess, entity.Verbose.Singular);

                        if (Request["ContinueEdit"] != null)
                        {
                            return RedirectToAction(
                                "Edit",
                                new
                                {
                                    entityName,
                                    key = entity.Key.Value.ToObject(savedId)
                                });
                        }
                        if (Request["AddNext"] != null)
                        {
                            return RedirectToAction("Create", new { entityName });
                        }

                        return RedirectToAction("Index", "Entities", new { entityName });
                    }
                    _notificator.Error(IlaroAdminResources.UncaughtError);
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "<br />" + ex.InnerException.Message;
                    }
                    _notificator.Error(message);
                }
            }

            var model = new EntityCreateModel
            {
                Entity = entity,
                PropertiesGroups = _entityService.PrepareGroups(entity)
            };

            return View(model);
        }

        public virtual ActionResult Edit(string entityName, string key)
        {
            var entity = _source.GetEntityWithData(entityName, key);
            if (entity == null)
            {
                return RedirectToAction("Index", "Entities", new { entityName });
            }

            entity.ClearPropertiesValues();
            var model = new EntityEditModel
            {
                Entity = entity,
                PropertiesGroups = _entityService.PrepareGroups(entity, false, key)
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Edit(
            string entityName,
            string key,
            FormCollection collection)
        {
            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }

            entity.Fill(collection, Request.Files);
            if (_validator.Validate(entity))
            {
                try
                {
                    var result = _entityService.Edit(entity);
                    if (result)
                    {
                        _notificator.Success(IlaroAdminResources.EditSuccess, entity.Verbose.Singular);


                        if (Request["ContinueEdit"] != null)
                        {
                            return RedirectToAction("Edit", new { entityName, key });
                        }
                        if (Request["AddNext"] != null)
                        {
                            return RedirectToAction("Create", new { entityName });
                        }

                        return RedirectToAction("Index", "Entities", new { entityName });
                    }
                    _notificator.Error(IlaroAdminResources.UncaughtError);
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "<br />" + ex.InnerException.Message;
                    }
                    _notificator.Error(message);
                }
            }

            var model = new EntityEditModel
            {
                Entity = entity,
                PropertiesGroups = _entityService.PrepareGroups(entity, false, key)
            };

            return View(model);
        }

        public virtual ActionResult Delete(string entityName, string key)
        {
            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }

            entity.Key.Value.Raw = key;

            var model = new EntityDeleteModel
            {
                Entity = entity,
                PropertiesDeleteOptions = entity.Properties
                    .Where(x =>
                        x.IsForeignKey &&
                        x.DeleteOption == DeleteOption.AskUser)
                    .Select(x =>
                        new PropertyDeleteOption
                        {
                            PropertyName = x.ForeignEntity.Name
                        })
                    .ToList(),
                RecordHierarchy = _hierarchySource.GetRecordHierarchy(entity)
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public virtual ActionResult Delete(EntityDeleteModel model)
        {
            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == model.EntityName);
            if (entity == null)
            {
                throw new NoNullAllowedException("entity is null");
            }

            try
            {
                var deleteOptions =
                    model.PropertiesDeleteOptions ??
                    new List<PropertyDeleteOption>();
                if (_entityService.Delete(entity, model.Key, deleteOptions))
                {
                    _notificator.Success(IlaroAdminResources.DeleteSuccess, entity.Verbose.Singular);

                    return RedirectToAction("Index", "Entities", new { entityName = model.EntityName });
                }

                _notificator.Error(IlaroAdminResources.UncaughtError);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "<br />" + ex.InnerException.Message;
                }
                _notificator.Error(message);
            }

            model = new EntityDeleteModel
            {
                Entity = entity,
                PropertiesDeleteOptions =
                    entity.Properties
                    .Where(x =>
                        x.IsForeignKey &&
                        x.DeleteOption == DeleteOption.AskUser)
                    .Select(x =>
                        new PropertyDeleteOption
                        {
                            PropertyName = x.ForeignEntity.Name
                        })
                    .ToList(),
                RecordHierarchy = _hierarchySource.GetRecordHierarchy(entity)
            };

            return View(model);
        }
    }
}
