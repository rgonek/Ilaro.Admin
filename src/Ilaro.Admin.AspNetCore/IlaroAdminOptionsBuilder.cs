using Dawn;
using SqlKata.Execution;
using System;

namespace Ilaro.Admin.AspNetCore
{
    public class IlaroAdminOptionsBuilder
    {
        internal string ConnectionString { get; private set; }

        internal Func<string, QueryFactory> QueryFactoryFactory { get; private set; }

        public void SetConnectionString(string connectionString)
        {
            Guard.Argument(connectionString, nameof(connectionString)).NotNull();

            ConnectionString = connectionString;
        }

        public void SetQueryFactoryFactory(Func<string, QueryFactory> queryFactoryFactory)
        {
            Guard.Argument(queryFactoryFactory, nameof(queryFactoryFactory)).NotNull();

            QueryFactoryFactory = queryFactoryFactory;
        }
    }
}
