using Ilaro.Admin.AspNetCore;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SqlClient;

namespace Ilaro.Admin.SqlServer
{
    public static class IlaroAdminOptionsBuilderExtensions
    {
        public static IlaroAdminOptionsBuilder UseSqlServer(this IlaroAdminOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder
                .SetConnectionString(connectionString)
                .SetQueryFactoryFactory(connectionString => new QueryFactory(new SqlConnection(connectionString), new SqlServerCompiler()));

            return optionsBuilder;
        }
    }
}
