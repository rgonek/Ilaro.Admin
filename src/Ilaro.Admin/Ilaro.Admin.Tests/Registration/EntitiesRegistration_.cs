using Ilaro.Admin.Tests.Scenarios.ScannedAssembly;
using Ilaro.Admin.Tests.Scenarios.ScannedAssembly.Models;
using System.Reflection;
using Xunit;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Tests.Registration
{
    public class EntitiesRegistration_
    {
        private readonly Assembly _testAssembly;

        public EntitiesRegistration_()
        {
            _testAssembly = typeof(Scenarios.ScannedAssembly.Entity).Assembly;
        }

        [Fact]
        public void when_registering_entities_from_assembly_without_any_constraints__all_types_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Register();

            Assert.NotNull(Admin.GetEntity<Car>());
            Assert.NotNull(Admin.GetEntity<Product>());
            Assert.NotNull(Admin.GetEntity<Category>());
            Assert.NotNull(Admin.GetEntity<Entity>());
            Assert.NotNull(Admin.GetEntity<Order>());
            Assert.NotNull(Admin.GetEntity<Role>());
            Assert.NotNull(Admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_except_constraints__all_types_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Except<Entity>()
                .Register();

            Assert.Null(Admin.GetEntity<Entity>());
            Assert.NotNull(Admin.GetEntity<Car>());
            Assert.NotNull(Admin.GetEntity<Product>());
            Assert.NotNull(Admin.GetEntity<Category>());
            Assert.NotNull(Admin.GetEntity<Order>());
            Assert.NotNull(Admin.GetEntity<Role>());
            Assert.NotNull(Admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_without_any_constraints__enums_should_not_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Register();

            Assert.Null(Admin.GetEntity<TestEnum>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_without_any_constraints__interfaces_should_not_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Register();

            Assert.Null(Admin.GetEntity<TestInterface>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_namespace_contraint__only_matching_entities_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Where(type => type.Namespace.EndsWith("Models"))
                .Register();

            Assert.NotNull(Admin.GetEntity<Car>());
            Assert.NotNull(Admin.GetEntity<Product>());
            Assert.Null(Admin.GetEntity<Category>());
            Assert.Null(Admin.GetEntity<Entity>());
            Assert.Null(Admin.GetEntity<Order>());
            Assert.Null(Admin.GetEntity<Role>());
            Assert.Null(Admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_namespace_of_contraint__only_matching_entities_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .InNamespaceOf<Car>()
                .Register();

            Assert.NotNull(Admin.GetEntity<Car>());
            Assert.NotNull(Admin.GetEntity<Product>());
            Assert.Null(Admin.GetEntity<Category>());
            Assert.Null(Admin.GetEntity<Entity>());
            Assert.Null(Admin.GetEntity<Order>());
            Assert.Null(Admin.GetEntity<Role>());
            Assert.Null(Admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_inheritance_contraint__only_matching_entities_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Where(type => type.IsSubclassOf(typeof(Entity)))
                .Register();

            Assert.NotNull(Admin.GetEntity<Category>());
            Assert.NotNull(Admin.GetEntity<Role>());
            Assert.Null(Admin.GetEntity<Car>());
            Assert.Null(Admin.GetEntity<Product>());
            Assert.Null(Admin.GetEntity<Entity>());
            Assert.Null(Admin.GetEntity<Order>());
            Assert.Null(Admin.GetEntity<User>());
        }
    }
}
