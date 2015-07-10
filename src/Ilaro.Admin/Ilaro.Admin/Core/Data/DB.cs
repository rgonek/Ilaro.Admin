using System.Configuration;
using System.Data.Common;

namespace Ilaro.Admin.Core.Data
{
    internal static class DB
    {
        internal static DbCommand CreateCommand(DbConnection conn = null)
        {
            var factory = GetFactory();
            var result = factory.CreateCommand();
            result.Connection = conn;
            return result;
        }

        internal static DbConnection OpenConnection()
        {
            var factory = GetFactory();
            var result = factory.CreateConnection();
            result.ConnectionString = GetConnectionString();
            result.Open();
            return result;
        }

        private static DbProviderFactory GetFactory()
        {
            var connectionStringName = Admin.ConnectionStringName;
            var providerName = "System.Data.SqlClient";

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;

            var factory = DbProviderFactories.GetFactory(providerName);

            return factory;
        }

        private static string GetConnectionString()
        {
            var connectionStringName = Admin.ConnectionStringName;
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            return connectionString;
        }
    }
}