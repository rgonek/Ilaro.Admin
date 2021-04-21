using Ilaro.Admin.AspNetCore;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Ilaro.Admin.PostgreSql
{
    public static class IlaroAdminOptionsBuilderExtensions
    {
        public static IlaroAdminOptionsBuilder UsePostgreSql(this IlaroAdminOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder
                .SetConnectionString(connectionString)
                .SetQueryFactoryFactory(connectionString => new QueryFactory(new NpgsqlConnection(connectionString), new PostgresCompiler()));

            return optionsBuilder;
        }
    }
}
