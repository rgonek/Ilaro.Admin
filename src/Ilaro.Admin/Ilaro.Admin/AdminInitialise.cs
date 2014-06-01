using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.WebPages;
using Ilaro.Admin.App_Start;
using RazorGenerator.Mvc;
using Ilaro.Admin.Extensions;
using System.Web.Mvc;

namespace Ilaro.Admin
{
	public static class AdminInitialise
	{
		public static IList<EntityViewModel> EntitiesTypes { get; set; }

		public static EntityViewModel ChangeEntity
		{
			get
			{
				return EntitiesTypes.FirstOrDefault(x => x.IsChangeEntity);
			}
		}

		public static IAuthorizationFilter Authorize { get; set; }

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
			routes.MapRoute(
				name: "IlaroAdminResources",
				url: "ira/{action}/{id}",
				defaults: new { controller = "IlaroAdminResource" }
			);
		}

		public static void RegisterRoutes(RouteCollection routes, string prefix = "IlaroAdmin")
		{
			routes.MapRoute(
				name: "IlaroAdminLogout",
				url: prefix + "/Logout",
				defaults: new { controller = "IlaroAdmin", action = "Logout" }
			);

			routes.MapRoute(
				name: "IlaroAdminCreate",
				url: prefix + "/{entityName}/Create",
				defaults: new { controller = "IlaroAdmin", action = "Create" }
			);

			routes.MapRoute(
				name: "IlaroAdminEdit",
				url: prefix + "/{entityName}/Edit/{key}",
				defaults: new { controller = "IlaroAdmin", action = "Edit" }
			);

			routes.MapRoute(
				name: "IlaroAdminDelete",
				url: prefix + "/{entityName}/Delete/{key}",
				defaults: new { controller = "IlaroAdmin", action = "Delete" }
			);

			routes.MapRoute(
				name: "IlaroAdminGroup",
				url: prefix + "/{groupName}/Group",
				defaults: new { controller = "IlaroAdmin", action = "Group" }
			);

			routes.MapRoute(
				name: "IlaroAdminChanges",
				url: prefix + "/{entityName}/Changes/{page}",
				defaults: new { controller = "IlaroAdmin", action = "Changes", page = 1 }
			);

			routes.MapRoute(
				name: "IlaroAdminList",
				url: prefix + "/{entityName}/{page}",
				defaults: new { controller = "IlaroAdmin", action = "List", page = 1 }
			);

			routes.MapRoute(
				name: "IlaroAdmin",
				url: prefix + "/{action}/{id}",
				defaults: new { controller = "IlaroAdmin", action = "Index", id = UrlParameter.Optional }
			);
		}

		public static void Initialise(string connectionString = "")
		{
			ConnectionString = connectionString;

			SetForeignKeysReferences();
		}

		public static void SetForeignKeysReferences()
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
			}

			foreach (var entity in EntitiesTypes)
			{
				foreach (var property in entity.Properties)
				{
					if (property.IsForeignKey)
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
							else if (!property.IsSystemType)
							{
								if (property.ForeignEntity != null)
								{
									property.PropertyType = property.ForeignEntity.Key.PropertyType;
								}
								else
								{
									// by default foreign property is int
									property.PropertyType = typeof(int);
								}
							}
						}
					}
				}

				foreach (var property in entity.Properties)
				{
					property.SetTemplatesName();
				}

				entity.SetColumns();
			}
		}
	}
}