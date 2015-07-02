using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Models;

namespace Ilaro.Admin
{
    public static class AdminInitialise
    {
        public static IList<Entity> EntitiesTypes { get; set; }

        public static Entity ChangeEntity
        {
            get
            {
                return EntitiesTypes.FirstOrDefault(x => x.IsChangeEntity);
            }
        }

        public static IAuthorizationFilter Authorize { get; set; }

        internal static string ConnectionString { get; private set; }

        public static string RoutesPrefix { get; private set; }

        static AdminInitialise()
        {
            EntitiesTypes = new List<Entity>();
        }

        public static Entity AddEntity<TEntity>()
        {
            var entity = new Entity(typeof(TEntity));
            EntitiesTypes.Add(entity);

            return entity;
        }

        public static void Initialise(string connectionString = "", string routesPrefix = "IlaroAdmin")
        {
            RoutesPrefix = routesPrefix;
            ConnectionString = connectionString;

            ModelBinders.Binders.Add(typeof(TableInfo),
                new TableInfoModelBinder(
                    (IConfiguration)DependencyResolver.Current.GetService(typeof(IConfiguration))));

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
                            else if (!property.TypeInfo.IsSystemType)
                            {
                                if (property.ForeignEntity != null)
                                {
                                    property.TypeInfo.Type = property.ForeignEntity.Key.TypeInfo.Type;
                                }
                                else
                                {
                                    // by default foreign property is int
                                    property.TypeInfo.Type = typeof(int);
                                }
                            }
                        }
                    }
                }

                foreach (var property in entity.Properties)
                {
                    property.Template = new PropertyTemplate(
                        property.Attributes,
                        property.TypeInfo,
                        property.IsForeignKey);
                }

                entity.SetColumns();
                entity.SetLinkKey();
                entity.PrepareGroups();
            }
        }
    }
}