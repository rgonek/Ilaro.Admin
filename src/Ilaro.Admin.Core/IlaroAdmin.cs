using Ilaro.Admin.Core.Configuration.Configurators;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public class IlaroAdmin : IIlaroAdmin
    {
        private EntitiesCollection _entitiesTypes = new EntitiesCollection();
        public ReadOnlyCollection<Entity> Entities
        {
            get
            {
                return _entitiesTypes.ToList().AsReadOnly();
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

        public IAuthorizeData Authorize { get; private set; }
        public string ConnectionStringName { get; private set; }
        public string RoutesPrefix { get; private set; }

        internal IDictionary<Type, IConfiguratorsHolder> CustomizerHolders { get; }
            = new Dictionary<Type, IConfiguratorsHolder>();

        public void RegisterEntity(Type entityType)
        {
            var configuratorsHolder = new ConfiguratorsHolder(entityType);
            Admin.AddConfigurator(configuratorsHolder);
        }

        public EntityConfigurator<TEntity> RegisterEntity<TEntity>() where TEntity : class
        {
            var configuratorsHolder = new ConfiguratorsHolder(typeof(TEntity));
            Admin.AddConfigurator(configuratorsHolder);
            return new EntityConfigurator<TEntity>(configuratorsHolder);
        }

        public Entity GetEntity(string entityName)
            => _entitiesTypes[entityName];

        public Entity GetEntity(Type type)
            => _entitiesTypes[type];

        public Entity GetEntity<TEntity>() where TEntity : class
            => GetEntity(typeof(TEntity));

        public void Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            IAuthorizeData authorize = null)
        {
            RoutesPrefix = routesPrefix;
            Authorize = authorize;
            RoutesPrefix = routesPrefix;
            ConnectionStringName = GetConnectionStringName(connectionStringName);

            foreach (var customizer in CustomizerHolders)
            {
                var entity = CreateInstance(customizer.Key);
                ((ConfiguratorsHolder)customizer.Value).CustomizeEntity(entity);
            }
            foreach (var customizer in CustomizerHolders)
            {
                var entity = GetEntity(customizer.Key);
                ((ConfiguratorsHolder)customizer.Value).CustomizeProperties(entity, _entitiesTypes);
            }
        }

        private Entity CreateInstance(Type entityType)
        {
            var entity = new Entity(entityType);
            _entitiesTypes.Add(entity);
            return entity;
        }

        private static string GetConnectionStringName(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                // TODO
                //if (ConfigurationManager.ConnectionStrings.Count > 1)
                //{
                //    connectionStringName = ConfigurationManager.ConnectionStrings[1].Name;
                //}
                //else
                {
                    throw new InvalidOperationException("Need a connection string name - can't determine what it is");
                }
            }

            return connectionStringName;
        }
    }
}