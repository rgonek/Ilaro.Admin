using Ilaro.Admin.Services.Interfaces;
using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.EntitiesFilters;
using Ilaro.Admin.Commons.Notificator;
using Resources;
using Ilaro.Admin.Attributes;
using System.Web.Security;

namespace Ilaro.Admin.Controllers
{
	[AuthorizeWrapper]
	public class IlaroAdminController : BaseController
	{
		private readonly IEntityService entityService;

		public IlaroAdminController(Notificator notificator, IEntityService entityService)
			: base(notificator)
		{
			this.entityService = entityService;
		}

		public ActionResult Index()
		{
			var viewModel = new IndexViewModel
			{
				EntitiesGroups = AdminInitialise.EntitiesTypes
								.GroupBy(x => x.GroupName)
								.Select(x => new EntityGroupViewModel
								{
									Name = x.Key,
									Entities = x.ToList()
								}).ToList()
			};

			return View(viewModel);
		}

		public ActionResult Group(string groupName)
		{
			var viewModel = new GroupViewModel
			{
				Group = AdminInitialise.EntitiesTypes
								.GroupBy(x => x.GroupName)
								.Where(x => x.Key == groupName)
								.Select(x => new EntityGroupViewModel
								{
									Name = x.Key,
									Entities = x.ToList()
								}).FirstOrDefault()
			};

			return View(viewModel);
		}

		public ActionResult List(string entityName, int page = 1, [Bind(Prefix = "sq")]string searchQuery = "", [Bind(Prefix = "pp")]int perPage = 10, [Bind(Prefix = "o")]string order = "", [Bind(Prefix = "od")]string orderDirection = "")
		{
			var entity = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName);
			var filters = entityService.PrepareFilters(entity, Request);
			var pagedRecords = entityService.GetRecords(entity, page, perPage, filters, searchQuery, order, orderDirection);
			if (pagedRecords.Records.IsNullOrEmpty() && page > 1)
			{
				return RedirectToAction("List", PrepareRouteValues(entityName, 1, perPage, filters, searchQuery, order, orderDirection));
			}

			var url = Url.Action("List", PrepareRouteValues(entityName, "-page-", perPage, filters, searchQuery, order, orderDirection)).Replace("-page-", "{0}");
			var viewModel = new ListViewModel
			{
				Data = pagedRecords.Records,
				Columns = entityService.PrepareColumns(entity, order, orderDirection),
				Entity = entity,
				PagerInfo = new PagerInfo(url, perPage, page, pagedRecords.TotalItems),
				Filters = filters,
				SearchQuery = searchQuery,
				IsSearchActive = entity.SearchProperties.Any(),
				PerPage = perPage,
				Order = order,
				OrderDirection = orderDirection
			};

			return View(viewModel);
		}

		public ActionResult Changes(string entityName, int page = 1, [Bind(Prefix = "sq")]string searchQuery = "", [Bind(Prefix = "pp")]int perPage = 10, [Bind(Prefix = "o")]string order = "", [Bind(Prefix = "od")]string orderDirection = "")
		{
			var entityChangesFor = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName);
			var changeEntity = AdminInitialise.ChangeEntity;
			var filters = entityService.PrepareFilters(changeEntity, Request);
			var pagedRecords = entityService.GetChangesRecords(entityChangesFor, page, perPage, filters, searchQuery, order, orderDirection);
			if (pagedRecords.Records.IsNullOrEmpty() && page > 1)
			{
				return RedirectToAction("Changes", PrepareRouteValues(entityName, 1, perPage, filters, searchQuery, order, orderDirection));
			}

			var url = Url.Action("Changes", PrepareRouteValues(entityName, "-page-", perPage, filters, searchQuery, order, orderDirection)).Replace("-page-", "{0}");
			var viewModel = new ChangesViewModel
			{
				Data = pagedRecords.Records,
				Columns = entityService.PrepareColumns(changeEntity, order, orderDirection),
				Entity = changeEntity,
				EntityChangesFor = entityChangesFor,
				PagerInfo = new PagerInfo(url, perPage, page, pagedRecords.TotalItems),
				Filters = filters,
				SearchQuery = searchQuery,
				IsSearchActive = changeEntity.SearchProperties.Any(),
				PerPage = perPage,
				Order = order,
				OrderDirection = orderDirection
			};

			return View(viewModel);
		}

		private RouteValueDictionary PrepareRouteValues(string entityName, int page, int perPage, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
		{
			return PrepareRouteValues(entityName, page.ToString(), perPage, filters, searchQuery, order, orderDirection);
		}

		private RouteValueDictionary PrepareRouteValues(string entityName, string page, int perPage, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
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

		public ActionResult Create(string entityName)
		{
			var entity = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName);
			entityService.ClearProperties(entity);

			var viewModel = new CreateViewModel
			{
				Entity = entity,
				PropertiesGroups = entityService.PrepareGroups(entity)
			};

			return View(viewModel);
		}

		[HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
		public ActionResult Create(string entityName, FormCollection collection)
		{
			var entity = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName);

			entityService.FillEntity(entity, collection);
			if (entityService.ValidateEntity(entity, ModelState))
			{
				try
				{
					var savedItem = entityService.Create(entity);
					if (savedItem != null)
					{
						Success(IlaroAdminResources.AddSuccess, entity.Singular);

						if (Request["ContinueEdit"] != null)
						{
							return RedirectToAction("Edit", new { entityName = entityName, key = entityService.GetKeyValue(entity, savedItem) });
						}
						else if (Request["AddNext"] != null)
						{
							return RedirectToAction("Create", new { entityName = entityName });
						}

						return RedirectToAction("List", new { entityName = entityName });
					}
					else
					{
						Error(IlaroAdminResources.UncaughtError);
					}
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

			var viewModel = new CreateViewModel
			{
				Entity = entity,
				PropertiesGroups = entityService.PrepareGroups(entity)
			};

			return View(viewModel);
		}

		public ActionResult Edit(string entityName, string key)
		{
			var entity = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName);

			try
			{
				entityService.FillEntity(entity, key);
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

				return RedirectToAction("List", new { entityName = entityName });
			}

			var viewModel = new EditViewModel
			{
				Entity = entity,
				PropertiesGroups = entityService.PrepareGroups(entity, false, key)
			};

			return View(viewModel);
		}

		[HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
		public ActionResult Edit(string entityName, string key, FormCollection collection)
		{
			var entity = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName);

			entityService.FillEntity(entity, collection);
			if (entityService.ValidateEntity(entity, ModelState))
			{
				try
				{
					var savedItems = entityService.Edit(entity);
					if (savedItems > 0)
					{
						Success(IlaroAdminResources.EditSuccess, entity.Singular);


						if (Request["ContinueEdit"] != null)
						{
							return RedirectToAction("Edit", new { entityName = entityName, key = key });
						}
						else if (Request["AddNext"] != null)
						{
							return RedirectToAction("Create", new { entityName = entityName });
						}

						return RedirectToAction("List", new { entityName = entityName });
					}
					else
					{
						Error(IlaroAdminResources.UncaughtError);
					}
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

			var viewModel = new EditViewModel
			{
				Entity = entity,
				PropertiesGroups = entityService.PrepareGroups(entity, false)
			};

			return View(viewModel);
		}

		public ActionResult Delete(string entityName, string key)
		{
			var entity = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName);
			entity.Key.Value = key;

			var viewModel = new DeleteViewModel
			{
				Entity = entity,
				PropertiesDeleteOptions = entity.Properties.Where(x => x.IsForeignKey && x.DeleteOption == DeleteOption.AskUser)
					.Select(x => new PropertyDeleteViewModel { PropertyName = x.ForeignEntity.Name }).ToList(),
				RecordHierarchy = entityService.GetRecordHierarchy(entity)
			};

			return View(viewModel);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult Delete(DeleteViewModel model)
		{
			var entity = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == model.EntityName);

			try
			{
				if (entityService.Delete(entity, model.Key, model.PropertiesDeleteOptions ?? new List<PropertyDeleteViewModel>()))
				{
					Success(IlaroAdminResources.DeleteSuccess, entity.Singular);

					return RedirectToAction("List", new { entityName = model.EntityName });
				}
				else
				{
					Error(IlaroAdminResources.UncaughtError);
				}
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

			var viewModel = new DeleteViewModel
			{
				Entity = entity,
				PropertiesDeleteOptions = entity.Properties.Where(x => x.IsForeignKey && x.DeleteOption == DeleteOption.AskUser)
					.Select(x => new PropertyDeleteViewModel { PropertyName = x.ForeignEntity.Name }).ToList(),
				RecordHierarchy = entityService.GetRecordHierarchy(entity)
			};

			return View(viewModel);
		}

		[ChildActionOnly]
		public virtual ActionResult Messages()
		{
			return PartialView("_Messages", notificator);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public virtual ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			return Redirect("/");
		}
	}
}
