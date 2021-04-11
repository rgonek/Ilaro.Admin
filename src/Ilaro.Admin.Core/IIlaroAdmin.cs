using Ilaro.Admin.Core.Configuration.Configurators;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.ObjectModel;

namespace Ilaro.Admin.Core
{
    public interface IIlaroAdmin
    {
        ReadOnlyCollection<Entity> Entities { get; }
        Entity ChangeEntity { get; }
        bool IsChangesEnabled { get; }

        IAuthorizeData Authorize { get; }
        string ConnectionStringName { get; }
        string RoutesPrefix { get; }

        void RegisterEntity(Type entityType);
        EntityConfigurator<TEntity> RegisterEntity<TEntity>() where TEntity : class;

        Entity GetEntity(string entityName);
        Entity GetEntity(Type type);
        Entity GetEntity<TEntity>() where TEntity : class;

        void Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            IAuthorizeData authorize = null);
    }
}