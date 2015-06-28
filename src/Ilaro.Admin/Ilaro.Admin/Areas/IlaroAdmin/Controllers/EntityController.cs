using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;
using Ilaro.Admin.Services.Interfaces;
using Resources;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class EntityController : BaseController
    {
        private readonly IEntityService _entityService;

        public EntityController(Notificator notificator, IEntityService entityService)
            : base(notificator)
        {
            _entityService = entityService;
        }

        public virtual ActionResult Create(string entityName)
        {
            ViewBag.IsAjaxRequest = HttpContext.Request.IsAjaxRequest();

            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
            _entityService.ClearProperties(entity);

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

            _entityService.FillEntity(entity, collection);
            if (_entityService.ValidateEntity(entity, ModelState))
            {
                try
                {
                    var savedItem = _entityService.Create(entity);
                    if (savedItem != null)
                    {
                        Success(IlaroAdminResources.AddSuccess, entity.Verbose.Singular);

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
                    Error(IlaroAdminResources.UncaughtError);
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "<br />" + ex.InnerException.Message;
                    }
                    Error(message);
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
            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);

            try
            {
                _entityService.FillEntity(entity, key);
                // catch error
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "<br />" + ex.InnerException.Message;
                }
                Error(message);

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

            _entityService.FillEntity(entity, collection);
            if (_entityService.ValidateEntity(entity, ModelState))
            {
                try
                {
                    var savedItems = _entityService.Edit(entity);
                    if (savedItems > 0)
                    {
                        Success(IlaroAdminResources.EditSuccess, entity.Verbose.Singular);


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
                    Error(IlaroAdminResources.UncaughtError);
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "<br />" + ex.InnerException.Message;
                    }
                    Error(message);
                }
            }

            var model = new EntityEditModel
            {
                Entity = entity,
                PropertiesGroups = _entityService.PrepareGroups(entity, false)
            };

            return View(model);
        }

        public virtual ActionResult Delete(string entityName, string key)
        {
            var entity = AdminInitialise.EntitiesTypes
                .FirstOrDefault(x => x.Name == entityName);
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

            try
            {
                var deleteOptions =
                    model.PropertiesDeleteOptions ??
                    new List<PropertyDeleteOption>();
                if (_entityService.Delete(entity, model.Key, deleteOptions))
                {
                    Success(IlaroAdminResources.DeleteSuccess, entity.Verbose.Singular);

                    return RedirectToAction("Index", "Entities", new { entityName = model.EntityName });
                }

                Error(IlaroAdminResources.UncaughtError);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "<br />" + ex.InnerException.Message;
                }
                Error(message);
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
