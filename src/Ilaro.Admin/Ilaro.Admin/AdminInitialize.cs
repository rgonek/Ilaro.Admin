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
				url: "ira/{action}/{id}",
				defaults: new { controller = "IlaroAdminResource" }
			);
		}

		public static void RegisterRoutes(RouteCollection routes, string prefix = "IlaroAdmin")
		{
			routes.MapRoute(
				name: "AdminCreate",
				url: prefix + "/Create/{entityName}",
				defaults: new { controller = "IlaroAdmin", action = "Create" }
			);

			routes.MapRoute(
				name: "AdminEdit",
				url: prefix + "/Edit/{entityName}/{key}",
				defaults: new { controller = "IlaroAdmin", action = "Edit" }
			);

			routes.MapRoute(
				name: "AdminDelete",
				url: prefix + "/Delete/{entityName}/{key}",
				defaults: new { controller = "IlaroAdmin", action = "Delete" }
			);

			routes.MapRoute(
				name: "AdminGroup",
				url: prefix + "/Group/{groupName}",
				defaults: new { controller = "IlaroAdmin", action = "Group" }
			);

			routes.MapRoute(
				name: "AdminDetails",
				url: prefix + "/{entityName}/{page}",
				defaults: new { controller = "IlaroAdmin", action = "Details", page = 1 }
			);

			routes.MapRoute(
				name: "Admin",
				url: prefix + "/{action}/{id}",
				defaults: new { controller = "IlaroAdmin", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}