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
        private readonly IFetchingEntitiesRecords _entitiesSource;
        private readonly IValidateEntity _validator;

        public EntityController(
            Notificator notificator,
            IEntityService entityService,
            IValidateEntity validator,
            IFetchingEntitiesRecords entitiesSource)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (entityService == null)
                throw new ArgumentNullException("entityService");
            if (validator == null)
                throw new ArgumentNullException("validator");
            if (entitiesSource == null)
                throw new ArgumentNullException("entitiesSource");

            _notificator = notificator;
            _entityService = entityService;
            _validator = validator;
            _entitiesSource = entitiesSource;
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

            entity.ClearProperties();

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
                    var savedItem = _entityService.Create(entity);
                    if (savedItem != null)
                    {
                        _notificator.Success(IlaroAdminResources.AddSuccess, entity.Verbose.Singular);

                        if (Request["ContinueEdit"] != null)
                        {
                            return RedirectToAction(
                                "Edit",
                                new
                                {
                                    entityName,
                                    key = _entityService
                                        .GetKeyValue(entity, savedItem)
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
            var entity = _entitiesSource.GetEntityWithData(entityName, key);
            try
            {
                //_entityService.FillEntity(entity, key);
                // catch error
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "<br />" + ex.InnerException.Message;
                }
                _notificator.Error(message);

                return RedirectToAction("Index", "Entities", new { entityName });
            }

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
                    var savedItems = _entityService.Edit(entity);
                    if (savedItems > 0)
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
                RecordHierarchy = _entityService.GetRecordHierarchy(entity)
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
                RecordHierarchy = _entityService.GetRecordHierarchy(entity)
            };

            return View(model);
        }
    }
}
