using System.Data.SqlClient;
using FakeItEasy;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class DbCommandExecutor_ : SqlServerDatabaseTest
    {
        private readonly IExecutingDbCommand _executor;

        public DbCommandExecutor_()
        {
            var user = A.Fake<IProvidingUser>();
            A.CallTo(() => user.Current()).Returns("Test");
            _executor = new DbCommandExecutor(user);
            Admin.ConnectionStringName = ConnectionStringName;
        }

        [Fact]
        public void does_not_create_entity_change_record_when_entity_change_is_not_added()
        {
            var cmd = new SqlCommand { CommandText = "SELECT 1;" };

            _executor.ExecuteWithChanges(cmd, "Product", EntityChangeType.Insert);

            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(0, changes.Count);
        }

        [Fact]
        public void create_entity_change_record_when_entity_change_is_added()
        {
            Admin.RegisterEntity<EntityChange>();
            var cmd = new SqlCommand { CommandText = "SELECT 1;" };

            _executor.ExecuteWithChanges(cmd, "Product", EntityChangeType.Insert);

            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }
    }
}
