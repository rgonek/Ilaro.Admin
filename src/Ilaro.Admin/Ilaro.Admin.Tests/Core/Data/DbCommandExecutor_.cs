using System.Data.SqlClient;
using FakeItEasy;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;
using Ilaro.Admin.Configuration;
using System.Collections.Generic;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class DbCommandExecutor_ : SqlServerDatabaseTest
    {
        private readonly IExecutingDbCommand _executor;

        public DbCommandExecutor_()
        {
            var user = A.Fake<IProvidingUser>();
            A.CallTo(() => user.CurrentUserName()).Returns("Test");
            _executor = new DbCommandExecutor(_admin, user);
        }

        [Fact]
        public void does_not_create_entity_change_record_when_entity_change_is_not_added()
        {
            Entity<Product>.Register();
            _admin.Initialise(ConnectionStringName);
            var cmd = new SqlCommand { CommandText = "SELECT 1;" };
            var entityRecord = create_product_entity_record();

            _executor.ExecuteWithChanges(cmd, entityRecord, EntityChangeType.Insert);

            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(0, changes.Count);
        }

        [Fact]
        public void create_entity_change_record_when_entity_change_is_added()
        {
            Entity<Product>.Register();
            Entity<EntityChange>.Register();
            _admin.Initialise(ConnectionStringName);
            var cmd = new SqlCommand { CommandText = "SELECT 1;" };
            var entityRecord = create_product_entity_record();

            _executor.ExecuteWithChanges(cmd, entityRecord, EntityChangeType.Insert);

            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }

        private EntityRecord create_product_entity_record()
        {
            var entityRecord = new EntityRecord(_admin.GetEntity<Product>());
            var values = new Dictionary<string, object>
            {
                { "ProductName", "Test" }
            };
            entityRecord.Fill(values);
            return entityRecord;
        }
    }
}
