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

        Entity RegisterEntity(Type entityType);
        Entity RegisterEntity<TEntity>() where TEntity : class;

        Entity GetEntity(string entityName);
        Entity GetEntity(Type type);
        Entity GetEntity<TEntity>() where TEntity : class;

        void Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            IAuthorizationFilter authorize = null);
    }
}