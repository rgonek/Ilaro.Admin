using System;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using System.Globalization;
using System.Reflection;
using Ilaro.Admin.Registration;

namespace Ilaro.Admin
{
    public static class Admin
    {
        public static IIlaroAdmin Current
        {
            get
            {
                return DependencyResolver.Current.GetService<IIlaroAdmin>();
            }
        }

        public static Entity RegisterEntity<TEntity>()
        {
            return RegisterEntity(typeof(TEntity));
        }

        public static Entity RegisterEntity(Type entityType)
        {
            return Current.RegisterEntity(entityType);
        }

        public static IIlaroAdmin Initialise(
            string connectionStringName = "",
            string routesPrefix = "IlaroAdmin",
            IAuthorizationFilter authorize = null,
            CultureInfo culture = null)
        {
            var admin = Current;
            admin.Initialise(connectionStringName, routesPrefix, authorize);

            return admin;
        }

        public static RegistrationBuilder AssemblyEntities(params Assembly[] assemblies)
        {
            return ScanningRegistrationExtensions.RegisterAssemblyTypes(assemblies);
        }

        public static RegistrationBuilder Entities(params Type[] types)
        {
            return ScanningRegistrationExtensions.RegisterTypes(types);
        }
    }
}