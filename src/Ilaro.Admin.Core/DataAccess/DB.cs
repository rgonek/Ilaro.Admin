﻿using System.Configuration;
using System.Data.Common;

namespace Ilaro.Admin.Core.DataAccess
{
    internal static class DB
    {
        internal static DbCommand CreateCommand(
            string connectionStringName,
            DbConnection conn = null)
        {
            var factory = GetFactory(connectionStringName);
            var result = factory.CreateCommand();
            result.Connection = conn;
            return result;
        }

        internal static DbConnection OpenConnection(string connectionStringName)
        {
            var factory = GetFactory(connectionStringName);
            var result = factory.CreateConnection();
            result.ConnectionString = GetConnectionString(connectionStringName);
            result.Open();
            return result;
        }

        private static DbProviderFactory GetFactory(string connectionStringName)
        {
            //var providerName = "System.Data.SqlClient";

            //if (!string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
            //    providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;

            var factory = System.Data.SqlClient.SqlClientFactory.Instance;// DbProviderFactories.GetFactory(providerName);

            return factory;
        }

        private static string GetConnectionString(string connectionStringName)
        {
            return "Server=.\\sql2017;initial catalog=Northwind;integrated security=SSPI";
            //var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            //return connectionString;
        }
    }
}