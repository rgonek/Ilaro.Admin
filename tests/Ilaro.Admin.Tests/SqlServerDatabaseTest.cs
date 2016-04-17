using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Ilaro.Admin.Tests.Utils;
using Simple.Data;

namespace Ilaro.Admin.Tests
{
    public class SqlServerDatabaseTest : TestBase
    {
        protected dynamic DB { get; private set; }

        protected string ConnectionStringName
        {
            get { return "IlaroTestDb"; }
        }

        public SqlServerDatabaseTest()
        {
            RecreateDatabase();

            DB = Database.OpenNamedConnection(ConnectionStringName);
        }

        private void RecreateDatabase()
        {
            var connectionString =
                ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DropAllObjects(connection);

                CreateDatabase(connection);
            }
        }

        protected virtual void CreateDatabase(DbConnection connection)
        {
            DatabaseCommandExecutor.ExecuteScript(
                  TestUtils.GetDatabaseScript(@"CreateDatabase.sql"),
                  connection
            );
        }

        protected virtual void DropAllObjects(IDbConnection connection)
        {
            DatabaseCommandExecutor.ExecuteScript(
                  TestUtils.GetDatabaseScript(@"DropAllObjects.sql"),
                  connection
            );
        }
    }
}
