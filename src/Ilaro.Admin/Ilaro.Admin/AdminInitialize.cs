using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace Ilaro.Admin
{
	public static class AdminInitialize
	{
		public static IList<EntityViewModel> EntitiesTypes { get; set; }

		public static DbContext Context { get; set; }

		static AdminInitialize()
		{
			EntitiesTypes = new List<EntityViewModel>();
		}

		public static void InitEntity<TEntity>()
		{
			EntitiesTypes.Add(new EntityViewModel(typeof(TEntity)));
		}

		public static void InitContext(DbContext context)
		{
			Context = context;
		}

		public static void RegisterResourceRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				name: "Resources",
				url: "ra/{action}/{id}",
				defaults: new { controller = "AdminsResource" }
			);
		}

		public static void RegisterRoutes(RouteCollection routes, string prefix = "Admin")
		{
			routes.MapRoute(
				name: "AdminsCreate",
				url: prefix + "/Create/{entityName}",
				defaults: new { controller = "Admins", action = "Create" }
			);

			routes.MapRoute(
				name: "AdminsEdit",
				url: prefix + "/Edit/{entityName}/{key}",
				defaults: new { controller = "Admins", action = "Edit" }
			);

			routes.MapRoute(
				name: "AdminsDelete",
				url: prefix + "/Delete/{entityName}/{key}",
				defaults: new { controller = "Admins", action = "Delete" }
			);

			routes.MapRoute(
				name: "AdminsGroup",
				url: prefix + "/Group/{groupName}",
				defaults: new { controller = "Admins", action = "Group" }
			);

			routes.MapRoute(
				name: "AdminsDetails",
				url: prefix + "/{entityName}/{page}",
				defaults: new { controller = "Admins", action = "Details", page = 1 }
			);

			routes.MapRoute(
				name: "Admins",
				url: prefix + "/{action}/{id}",
				defaults: new { controller = "Admins", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}