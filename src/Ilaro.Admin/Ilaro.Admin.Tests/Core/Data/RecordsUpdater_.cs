using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;
using Ilaro.Admin.Configuration;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsUpdater_ : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private readonly IUpdatingRecords _updater;
        private readonly IProvidingUser _user;
        private EntityRecord _entityRecord;
        private int _productId;

        public RecordsUpdater_()
        {
            _source = new RecordsSource(_admin, new Notificator());
            _user = A.Fake<IProvidingUser>();
            A.CallTo(() => _user.CurrentUserName()).Returns("Test");
            var executor = new DbCommandExecutor(_admin, _user);
            _updater = new RecordsUpdater(_admin, executor, _source, _user);
        }

        [Fact]
        public void updates_record_and_does_not_create_entity_change_when_is_not_added()
        {
            set_up_test();

            _entityRecord["ProductName"].Raw = "Product2";
            _updater.Update(_entityRecord);

            var products = (List<Product>)DB.Products.All();
            Assert.Equal(1, products.Count);
            Assert.Equal("Product2", products.First().ProductName);

            A.CallTo(() => _user.CurrentUserName()).MustNotHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(0, changes.Count);
        }

        [Fact]
        public void updates_record_and_does_create_entity_change_when_is_added()
        {
            _admin.RegisterEntity<EntityChange>();
            set_up_test();

            _entityRecord["ProductName"].Raw = "Product2";
            _updater.Update(_entityRecord);

            var products = (List<Product>)DB.Products.All();
            Assert.Equal(1, products.Count);
            Assert.Equal("Product2", products.First().ProductName);

            A.CallTo(() => _user.CurrentUserName()).MustHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }

        [Fact]
        public void updates_record_with_one_to_many_foreign_property()
        {
            set_up_test();
            var categoryId = DB.Categories.Insert(CategoryName: "Category").CategoryID;

            _entityRecord["ProductName"].Raw = "Product2";
            _entityRecord["Discontinued"].Raw = false;
            _entityRecord["Category"].Raw = categoryId;
            _updater.Update(_entityRecord);

            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(1, products.Count);
            Assert.Equal(categoryId, products.First().CategoryID);
        }

        [Fact]
        public void updates_record_with_many_to_one_foreign_property()
        {
            set_up_test();
            var category = DB.Categories.Insert(CategoryName: "Category");
            var product2 = DB.Products.Insert(ProductName: "Product2", CategoryId: category.CategoryID);

            _entityRecord = _source.GetEntityRecord(
                _admin.GetEntity<Category>(),
                category.CategoryID.ToString());

            _entityRecord["CategoryName"].Raw = "Category";
            _entityRecord["Products"].Values = new List<object> { _productId };
            _updater.Update(_entityRecord);

            var categories = (List<dynamic>)DB.Categories.All().ToList();
            Assert.Equal(1, categories.Count);
            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(2, products.Count);
            var product = products.First(x => x.ProductID == _productId);
            product2 = products.First(x => x.ProductID == product2.ProductID);
            Assert.Null(product2.CategoryID);
            Assert.Equal(category.CategoryID, product.CategoryID);
        }

        private void set_up_test()
        {
            Entity<Product>.Register().ReadAttributes();
            Entity<Category>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);

            _productId = DB.Products.Insert(ProductName: "Product").ProductID;
            _entityRecord = _source.GetEntityRecord(
                _admin.GetEntity<Product>(), 
                _productId.ToString());
        }
    }
}
