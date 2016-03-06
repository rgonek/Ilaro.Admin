using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsCreator_ : SqlServerDatabaseTest
    {
        private readonly ICreatingRecords _creator;
        private readonly IProvidingUser _user;
        private Entity _entity;

        public RecordsCreator_()
        {
            SetFakeResolver();

            _user = A.Fake<IProvidingUser>();
            A.CallTo(() => _user.Current()).Returns("Test");
            var executor = new DbCommandExecutor(_user);
            _creator = new RecordsCreator(executor);
            Admin.RegisterEntity<Product>();
            Admin.RegisterEntity<Category>();
            Admin.Initialise(ConnectionStringName);
            _entity = Admin.GetEntity("Product");
        }

        [Fact]
        public void creates_record_and_does_not_create_entity_change_when_is_not_added()
        {
            _entity["ProductName"].Value.Raw = "Product";
            _entity["Discontinued"].Value.Raw = false;
            _creator.Create(_entity);

            var products = DB.Products.All().ToList();
            Assert.Equal(1, products.Count);

            A.CallTo(() => _user.Current()).MustNotHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(0, changes.Count);
        }

        [Fact]
        public void creates_record_and_does_create_entity_change_when_is_added()
        {
            Admin.RegisterEntity<EntityChange>();
            _entity["ProductName"].Value.Raw = "Product";
            _entity["Discontinued"].Value.Raw = false;
            _creator.Create(_entity);

            var products = DB.Products.All().ToList();
            Assert.Equal(1, products.Count);

            A.CallTo(() => _user.Current()).MustHaveHappened();
            var changes = DB.EntityChanges.All().ToList();
            Assert.Equal(1, changes.Count);
        }

        [Fact]
        public void creates_record_with_one_to_many_foreign_property()
        {
            var categoryId = DB.Categories.Insert(CategoryName: "Category").CategoryID;
            Admin.RegisterEntity<Category>();
            _entity["ProductName"].Value.Raw = "Product";
            _entity["Discontinued"].Value.Raw = false;
            _entity["Category"].Value.Raw = categoryId;
            _creator.Create(_entity);

            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(1, products.Count);
            Assert.Equal(categoryId, products.First().CategoryID);
        }

        [Fact]
        public void creates_record_with_many_to_one_foreign_property()
        {
            var productId = DB.Products.Insert(ProductName: "Product").ProductID;
            _entity = Admin.GetEntity("Category");
            _entity["CategoryName"].Value.Raw = "Category";
            _entity["Products"].Value.Values.Add(productId);
            _creator.Create(_entity);

            var categories = (List<dynamic>)DB.Categories.All().ToList();
            Assert.Equal(1, categories.Count);
            var products = (List<dynamic>)DB.Products.All().ToList();
            Assert.Equal(1, products.Count);
            Assert.Equal(categories.First().CategoryID, products.First().CategoryID);
        }
    }
}
