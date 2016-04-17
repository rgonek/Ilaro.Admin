using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Xunit;

namespace Ilaro.Admin.Tests.Core
{
    public class Property_DetermineForeignKey : TestBase
    {
        private readonly Entity _entity;

        public Property_DetermineForeignKey()
        {
            Entity<TestEntity>.Register().ReadAttributes();
            _admin.Initialise();
            _entity = _admin.GetEntity<TestEntity>();
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Name")]
        [InlineData("IsSpecial")]
        [InlineData("DateAdd")]
        [InlineData("Price")]
        [InlineData("Percent")]
        [InlineData("ParentId")]
        public void simple_types_without_foreign_attribute__are_not_foreign_key(string propertyName)
        {
            var property = _entity[propertyName];
            Assert.NotNull(property);
            Assert.False(property.IsForeignKey);
        }

        [Theory]
        [InlineData("Tags")]
        [InlineData("Dimensions")]
        [InlineData("IsSpecial")]
        [InlineData("DateAdd")]
        [InlineData("Price")]
        [InlineData("Percent")]
        public void collections_of_simple_types_without_foreign_attribute__are_not_foreign_key(string propertyName)
        {
            var property = _entity[propertyName];
            Assert.NotNull(property);
            Assert.False(property.IsForeignKey);
        }

        [Theory]
        [InlineData("Siblings")]
        public void collections_of_entity_types__are_foreign_key(string propertyName)
        {
            var property = _entity[propertyName];
            Assert.NotNull(property);
            Assert.True(property.IsForeignKey);
        }

        [Theory]
        [InlineData("Option")]
        [InlineData("SplitOption")]
        public void enums__are_not_foreign_key(string propertyName)
        {
            var property = _entity[propertyName];
            Assert.NotNull(property);
            Assert.False(property.IsForeignKey);
        }

        [Theory]
        [InlineData("Parent")]
        [InlineData("Child")]
        public void complex_types__are_foreign_key(string propertyName)
        {
            var property = _entity[propertyName];
            Assert.NotNull(property);
            Assert.True(property.IsForeignKey);
        }

        [Theory]
        [InlineData("RoleId")]
        public void simple_types_marked_with_foreign_attribute__are_foreign_key(string propertyName)
        {
            var property = _entity[propertyName];
            Assert.NotNull(property);
            Assert.True(property.IsForeignKey);
        }

        [Theory]
        [InlineData("ChildId")]
        public void simple_types_mentioned_in_foreign_attribute_of_other_property__are_foreign_key(string propertyName)
        {
            var property = _entity[propertyName];
            Assert.NotNull(property);
            Assert.True(property.IsForeignKey);
        }
    }
}
