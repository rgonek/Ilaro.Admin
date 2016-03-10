using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Ilaro.Admin
{
    public class IlaroAdmin : IIlaroAdmin
    {
        private List<Entity> _entitiesTypes = new List<Entity>();
        public ReadOnlyCollection<Entity> Entities
        {
            get
            {
                return _entitiesTypes.AsReadOnly();
            }
        }

        public Entity ChangeEntity
        {
            get
            {
                return _entitiesTypes.FirstOrDefault(x => x.IsChangeEntity);
            }
        }
        public bool IsChangesEnabled
        {
            get { return ChangeEntity != null; }
        }

        public IAuthorizationFilter Authorize { get; private set; }
        public string ConnectionStringName { get; private set; }
        public string RoutesPrefix { get; private set; }

        public Entity RegisterEntity(Type entityType)
        {
            var entity = new Entity(entityType);
            _entitiesTypes.Add(entity);

            return entity;
        }

        public Entity RegisterEntity<TEntity>()
        {
            return RegisterEntity(typeof(TEntity));
        }

        public Entity GetEntity(string entityName)
        {
            return _entitiesTypes.FirstOrDefault(x => x.Name == entityName);
        }

        public Entity GetEntity<TEntity>()
        {
            return _entitiesTypes.FirstOrDefault(x => x.Type == typeof(TEntity));
        }

        public void Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            IAuthorizationFilter authorize = null)
        {
            RoutesPrefix = routesPrefix;
            Authorize = authorize;
            RoutesPrefix = routesPrefix;
            ConnectionStringName = GetConnectionStringName(connectionStringName);

            SetForeignKeysReferences();
        }

        private static string GetConnectionStringName(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                if (ConfigurationManager.ConnectionStrings.Count > 1)
                {
                    connectionStringName = ConfigurationManager.ConnectionStrings[1].Name;
                }
                else
                {
                    throw new InvalidOperationException("Need a connection string name - can't determine what it is");
                }
            }

            return connectionStringName;
        }

        private void SetForeignKeysReferences()
        {
            foreach (var entity in _entitiesTypes)
            {
                // Try determine which property is a entity key if is not set
                if (entity.Key.IsNullOrEmpty())
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

            foreach (var entity in _entitiesTypes)
            {
                foreach (var property in entity.Properties)
                {
                    if (property.IsForeignKey)
                    {
                        property.ForeignEntity = _entitiesTypes.FirstOrDefault(x => x.Name == property.ForeignEntityName);

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
                                    property.TypeInfo.Type = property.ForeignEntity.Key.FirstOrDefault().TypeInfo.Type;
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