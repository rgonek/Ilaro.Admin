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
            A.CallTo(() => _user.CurrentUserName()).Returns("Test");
            var executor = new DbCommandExecutor(_admin, _user);
            _creator = new RecordsCreator(_admin, executor, _user);
        }

        [Fact]
        public void creates_record_and_does_not_create_entity_change_when_is_not_added()
        {
            register_default_entities();

            var values = new Dictionary<string, object>
            {
                { "ProductName", "Product" },
                { "Discontinued", false }
            };
            var entityRecord = new EntityRecord(_productEntity);
            entityRecord.Fill(values);
            _creator.Create(entityRecord);

            var products = DB.Products.All().ToList();
            Assert.Equal(1, products.Count);

            A.CallTo(() => _user.CurrentUserName()).MustNotHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(0, changes.Count);
        }

        [Fact]
        public void creates_record_and_does_create_entity_change_when_is_added()
        {
            Entity<EntityChange>.Register();
            register_default_entities();

            var values = new Dictionary<string, object>
            {
                { "ProductName", "Product" },
                { "Discontinued", false }
            };
            var entityRecord = new EntityRecord(_productEntity);
            entityRecord.Fill(values);
            _creator.Create(entityRecord);

            var products = DB.Products.All().ToList();
            Assert.Equal(1, products.Count);

            A.CallTo(() => _user.CurrentUserName()).MustHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }

        [Fact]
        public void creates_record_with_one_to_many_foreign_property()
        {
            Entity<Category>.Register();
            register_default_entities();

            var categoryId = DB.Categories.Insert(CategoryName: "Category").CategoryID;

            var values = new Dictionary<string, object>
            {
                { "ProductName", "Product" },
                { "Discontinued", false },
                { "CategoryID", categoryId }
            };
            var entityRecord = new EntityRecord(_productEntity);
            entityRecord.Fill(values);
            _creator.Create(entityRecord);

            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(1, products.Count);
            Assert.Equal(categoryId, products.First().CategoryID);
        }

        [Fact]
        public void creates_record_with_many_to_one_foreign_property()
        {
            register_default_entities();

            var productId = DB.Products.Insert(ProductName: "Product").ProductID;
            var categoryEntity = _admin.GetEntity<Category>();

            var entityRecord = new EntityRecord(categoryEntity);
            entityRecord.Fill(new Dictionary<string, object>());
            entityRecord["CategoryName"].Raw = "Category";
            entityRecord["Products"].Values.Add(productId);
            _creator.Create(entityRecord);

            var categories = (List<dynamic>)DB.Categories.All().ToList();
            Assert.Equal(1, categories.Count);
            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(1, products.Count);
            Assert.Equal(categories.First().CategoryID, products.First().CategoryID);
        }

        private void register_default_entities()
        {
            Entity<Product>.Register().ReadAttributes();
            Entity<Category>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);
            _productEntity = _admin.GetEntity<Product>();
        }
    }
}
