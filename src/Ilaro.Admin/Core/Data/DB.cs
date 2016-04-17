using System.Configuration;
using System.Data.Common;

namespace Ilaro.Admin.Core.Data
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
            var providerName = "System.Data.SqlClient";

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;

            var factory = DbProviderFactories.GetFactory(providerName);

            return factory;
        }

        private static string GetConnectionString(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            return connectionString;
        }
    }
}