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
        private readonly IIlaroAdmin _admin;
        private readonly IExecutingDbCommand _executor;

        public DbCommandExecutor_()
        {
            _admin = new IlaroAdmin();

            var user = A.Fake<IProvidingUser>();
            A.CallTo(() => user.Current()).Returns("Test");
            _executor = new DbCommandExecutor(_admin, user);
            _admin.Initialise(ConnectionStringName);
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
            _admin.RegisterEntity<EntityChange>();
            var cmd = new SqlCommand { CommandText = "SELECT 1;" };

            _executor.ExecuteWithChanges(cmd, "Product", EntityChangeType.Insert);

            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }
    }
}
