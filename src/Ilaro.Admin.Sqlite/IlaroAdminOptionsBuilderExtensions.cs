using Ilaro.Admin.AspNetCore;
using Microsoft.Data.Sqlite;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Ilaro.Admin.Sqlite
{
    public static class IlaroAdminOptionsBuilderExtensions
    {
        public static IlaroAdminOptionsBuilder UseSqlite(this IlaroAdminOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.SetConnectionString(connectionString);
            optionsBuilder.SetQueryFactoryFactory(connectionString => new QueryFactory(new SqliteConnection(connectionString), new SqliteCompiler()));

            return optionsBuilder;
        }
    }
}
