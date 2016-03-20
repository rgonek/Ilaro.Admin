using Ilaro.Admin.Configuration.Customizers;
using Ilaro.Admin.Core;
using System;
using System.Collections.ObjectModel;
using System.Web.Mvc;

namespace Ilaro.Admin
{
    public interface IIlaroAdmin
    {
        ReadOnlyCollection<Entity> Entities { get; }
        Entity ChangeEntity { get; }
        bool IsChangesEnabled { get; }

        IAuthorizationFilter Authorize { get; }
        string ConnectionStringName { get; }
        string RoutesPrefix { get; }

        void RegisterEntity(Type entityType);
        EntityCustomizer<TEntity> RegisterEntity<TEntity>() where TEntity : class;

        void RegisterEntityWithAttributes(Type entityType);
        EntityCustomizer<TEntity> RegisterEntityWithAttributes<TEntity>() where TEntity : class;

        Entity GetEntity(string entityName);
        Entity GetEntity(Type type);
        Entity GetEntity<TEntity>() where TEntity : class;

        void Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            IAuthorizationFilter authorize = null);
    }
}