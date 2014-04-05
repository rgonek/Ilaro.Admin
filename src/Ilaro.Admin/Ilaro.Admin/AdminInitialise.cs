using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.WebPages;
using Ilaro.Admin.App_Start;
using RazorGenerator.Mvc;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin
{
	public static class AdminInitialise
	{
		public static IList<EntityViewModel> EntitiesTypes { get; set; }

		internal static string ConnectionString { get; set; }

		static AdminInitialise()
		{
			EntitiesTypes = new List<EntityViewModel>();
		}

		public static void AddEntity<TEntity>()
		{
			EntitiesTypes.Add(new EntityViewModel(typeof(TEntity)));
		}

		public static void RegisterResourceRoutes(RouteCollection routes)
		{
			//routes.MapRoute(
			//    name: "Resources",
			//    url: "ira/{action}/{id}",
			//    defaults: new { controller = "IlaroAdminResource" }
			//);
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

		public static void Initialise(string connectionString = "")
		{
			ConnectionString = connectionString;

			Bootstrapper.Initialise();

			SetForeignKeysReferences();
		}

		private static void SetForeignKeysReferences()
		{
			foreach (var entity in EntitiesTypes)
			{
				// Try determine which property is a entity key if is not setted
				if (entity.Key == null)
				{
					var entityKey = entity.Properties.FirstOrDefault(x => x.Name.ToLower() == "id");
					if (entityKey == null)
					{
						entityKey = entity.Properties.FirstOrDefault(x => x.Name.ToLower() == entity.Name.ToLower() + "id");
						if (entityKey == null)
						{
							throw new Exception("Entity does not have a defined key");
						}
					}

					entityKey.IsKey = true;
					if (entity.LinkKey == null)
					{
						entityKey.IsLinkKey = true;
					}
				}

				foreach (var property in entity.Properties.Where(x => x.IsForeignKey))
				{
					property.ForeignEntity = EntitiesTypes.FirstOrDefault(x => x.Name == property.ForeignEntityName);

					if (!property.ReferencePropertyName.IsNullOrEmpty())
					{
						property.ReferenceProperty = entity.Properties.FirstOrDefault(x => x.Name == property.ReferencePropertyName);
						if (property.ReferenceProperty != null)
						{
							property.ReferenceProperty.IsForeignKey = true;
							property.ReferenceProperty.ForeignEntity = property.ForeignEntity;
							property.ReferenceProperty.ReferenceProperty = property;
						}
					}
				}
			}
		}
	}
}