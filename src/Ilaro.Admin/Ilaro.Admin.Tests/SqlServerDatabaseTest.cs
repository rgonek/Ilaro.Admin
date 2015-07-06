using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Ilaro.Admin.Tests.Utils;

namespace Ilaro.Admin.Tests
{
    public class SqlServerDatabaseTest
    {
        public SqlServerDatabaseTest()
        {
            RecreateDatabase();
        }

        private void RecreateDatabase()
        {
            var connectionString =
                ConfigurationManager.ConnectionStrings["IlaroTestDb"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                DropAllObjects(connection);

                CreateDatabase(connection);
            }
        }

        private void CreateDatabase(DbConnection connection)
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
