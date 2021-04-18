using Ilaro.Admin.AspNetCore;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Ilaro.Admin.MySql
{
    public static class IlaroAdminOptionsBuilderExtensions
    {
        public static IlaroAdminOptionsBuilder UseMySql(this IlaroAdminOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.SetConnectionString(connectionString);
            optionsBuilder.SetQueryFactoryFactory(connectionString => new QueryFactory(new MySqlConnection(connectionString), new MySqlCompiler()));

            return optionsBuilder;
        }
    }
}
