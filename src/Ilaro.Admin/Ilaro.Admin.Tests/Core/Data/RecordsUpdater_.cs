using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsUpdater_ : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private readonly IUpdatingRecords _updater;
        private readonly IProvidingUser _user;
        private Entity _entity;

        public RecordsUpdater_()
        {
            _source = new RecordsSource(new Notificator());
            _user = A.Fake<IProvidingUser>();
            A.CallTo(() => _user.Current()).Returns("Test");
            var executor = new DbCommandExecutor(_user);
            _updater = new RecordsUpdater(new Notificator(), executor, _source);
            AdminInitialise.AddEntity<Product>();
            AdminInitialise.SetForeignKeysReferences();
            AdminInitialise.ConnectionStringName = ConnectionStringName;

            var productId = DB.Products.Insert(ProductName: "Product").ProductID;
            _entity = _source.GetEntityWithData("Product", productId.ToString());
        }

        [Fact]
        public void updates_record_and_does_not_create_entity_change_when_is_not_added()
        {
            _entity["ProductName"].Value.Raw = "Product2";
            _updater.Update(_entity);

            var products = (List<Product>)DB.Products.All();
            Assert.Equal(1, products.Count);
            Assert.Equal("Product2", products.First().ProductName);

            A.CallTo(() => _user.Current()).MustNotHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(0, changes.Count);
        }

        [Fact]
        public void creates_record_and_does_create_entity_change_when_is_added()
        {
            AdminInitialise.AddEntity<EntityChange>();
            _entity["ProductName"].Value.Raw = "Product2";
            _updater.Update(_entity);

            var products = (List<Product>)DB.Products.All();
            Assert.Equal(1, products.Count);
            Assert.Equal("Product2", products.First().ProductName);

            A.CallTo(() => _user.Current()).MustHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }
    }
}
