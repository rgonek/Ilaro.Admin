using FirebirdSql.Data.FirebirdClient;
using Ilaro.Admin.AspNetCore;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Ilaro.Admin.Firebird
{
    public static class IlaroAdminOptionsBuilderExtensions
    {
        public static IlaroAdminOptionsBuilder UseFirebird(this IlaroAdminOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.SetConnectionString(connectionString);
            optionsBuilder.SetQueryFactoryFactory(connectionString => new QueryFactory(new FbConnection(connectionString), new FirebirdCompiler()));

            return optionsBuilder;
        }
    }
}
