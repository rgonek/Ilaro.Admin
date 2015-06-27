using System.Linq;
using Ilaro.Admin.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ilaro.Admin.Tests
{
    [TestClass]
    public class PropertyViewModelTests
    {
        [TestMethod]
        public void SetForeigns_SimpleTypes_IsNotForeignKey()
        {
            var entityType = typeof(TestEntity);
            var entity = new Entity(entityType);

            var property = entity["Id"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["Name"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["IsSpecial"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["DateAdd"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["Price"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["Percent"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);
        }

        [TestMethod]
        public void SetForeigns_Collections_IsNotForeignKey()
        {
            var entityType = typeof(TestEntity);
            var entity = new Entity(entityType);

            var property = entity["Tags"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["Dimensions"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);
        }

        [TestMethod]
        public void SetForeigns_Collections_IsForeignKey()
        {
            var entityType = typeof(TestEntity);
            var entity = new Entity(entityType);

            var property = entity["Siblings"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);
        }

        [TestMethod]
        public void SetForeigns_Enums_IsNotForeignKey()
        {
            var entityType = typeof(TestEntity);
            var entity = new Entity(entityType);

            var property = entity["Option"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["SplitOption"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);
        }

        [TestMethod]
        public void SetForeigns_ComplexTypes_IsForeignKey()
        {
            var entityType = typeof(TestEntity);
            var entity = new Entity(entityType);

            var property = entity["Parent"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);

            property = entity["Child"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);
        }

        [TestMethod]
        public void SetForeigns_SimpleTypForeignAttribute_IsForeignKey()
        {
            var entityType = typeof(TestEntity);
            var entity = new Entity(entityType);

            var property = entity["RoleId"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);
        }

        [TestMethod]
        public void SetForeigns_ForeignAttribute_SetForeignKeysReferences()
        {
            AdminInitialise.AddEntity<TestEntity>();
            AdminInitialise.SetForeignKeysReferences();

            var entity = AdminInitialise.EntitiesTypes.FirstOrDefault();
            Assert.IsNotNull(entity);

            var property = entity["Parent"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);

            property = entity["ParentId"];
            Assert.IsNotNull(property);
            Assert.IsFalse(property.IsForeignKey);

            property = entity["Child"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);

            property = entity["ChildId"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);

            property = entity["RoleId"];
            Assert.IsNotNull(property);
            Assert.IsTrue(property.IsForeignKey);
        }
    }
}
