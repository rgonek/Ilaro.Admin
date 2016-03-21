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
    public class RecordsCreator_ : SqlServerDatabaseTest
    {
        private readonly ICreatingRecords _creator;
        private readonly IProvidingUser _user;
        private Entity _productEntity;

        public RecordsCreator_()
        {
            _user = A.Fake<IProvidingUser>();
            A.CallTo(() => _user.Current()).Returns("Test");
            var executor = new DbCommandExecutor(_admin, _user);
            _creator = new RecordsCreator(_admin, executor);
        }

        [Fact]
        public void creates_record_and_does_not_create_entity_change_when_is_not_added()
        {
            register_default_entities();

            _productEntity["ProductName"].Value.Raw = "Product";
            _productEntity["Discontinued"].Value.Raw = false;
            _creator.Create(_productEntity);

            var products = DB.Products.All().ToList();
            Assert.Equal(1, products.Count);

            A.CallTo(() => _user.Current()).MustNotHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(0, changes.Count);
        }

        [Fact]
        public void creates_record_and_does_create_entity_change_when_is_added()
        {
            Entity<EntityChange>.Register();
            register_default_entities();

            _productEntity["ProductName"].Value.Raw = "Product";
            _productEntity["Discontinued"].Value.Raw = false;
            _creator.Create(_productEntity);

            var products = DB.Products.All().ToList();
            Assert.Equal(1, products.Count);

            A.CallTo(() => _user.Current()).MustHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }

        [Fact]
        public void creates_record_with_one_to_many_foreign_property()
        {
            Entity<Category>.Register();
            register_default_entities();

            var categoryId = DB.Categories.Insert(CategoryName: "Category").CategoryID;

            _productEntity["ProductName"].Value.Raw = "Product";
            _productEntity["Discontinued"].Value.Raw = false;
            _productEntity["Category"].Value.Raw = categoryId;
            _creator.Create(_productEntity);

            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(1, products.Count);
            Assert.Equal(categoryId, products.First().CategoryID);
        }

        [Fact]
        public void creates_record_with_many_to_one_foreign_property()
        {
            register_default_entities();

            var productId = DB.Products.Insert(ProductName: "Product").ProductID;
            _productEntity = _admin.GetEntity("Category");
            _productEntity["CategoryName"].Value.Raw = "Category";
            _productEntity["Products"].Value.Values.Add(productId);
            _creator.Create(_productEntity);

            var categories = (List<dynamic>)DB.Categories.All().ToList();
            Assert.Equal(1, categories.Count);
            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(1, products.Count);
            Assert.Equal(categories.First().CategoryID, products.First().CategoryID);
        }

        private void register_default_entities()
        {
            Entity<Product>.RegisterWithAttributes();
            Entity<Category>.RegisterWithAttributes();
            _admin.Initialise(ConnectionStringName);
            _productEntity = _admin.GetEntity<Product>();
        }
    }
}
