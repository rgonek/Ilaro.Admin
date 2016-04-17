using Ilaro.Admin.Tests.Scenarios.ScannedAssembly;
using Ilaro.Admin.Tests.Scenarios.ScannedAssembly.Models;
using System.Reflection;
using Xunit;
using Ilaro.Admin.Sample.Configurators;

namespace Ilaro.Admin.Tests.Registration
{
    public class EntitiesRegistration_ : TestBase
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
            _admin.Initialise();

            Assert.NotNull(_admin.GetEntity<Car>());
            Assert.NotNull(_admin.GetEntity<Product>());
            Assert.NotNull(_admin.GetEntity<Category>());
            Assert.NotNull(_admin.GetEntity<Entity>());
            Assert.NotNull(_admin.GetEntity<Order>());
            Assert.NotNull(_admin.GetEntity<Role>());
            Assert.NotNull(_admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_except_constraints__all_types_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Except<Entity>()
                .Register();
            _admin.Initialise();

            Assert.Null(_admin.GetEntity<Entity>());
            Assert.NotNull(_admin.GetEntity<Car>());
            Assert.NotNull(_admin.GetEntity<Product>());
            Assert.NotNull(_admin.GetEntity<Category>());
            Assert.NotNull(_admin.GetEntity<Order>());
            Assert.NotNull(_admin.GetEntity<Role>());
            Assert.NotNull(_admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_without_any_constraints__enums_should_not_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Register();
            _admin.Initialise();

            Assert.Null(_admin.GetEntity(typeof(TestEnum)));
        }

        [Fact]
        public void when_registering_entities_from_assembly_without_any_constraints__interfaces_should_not_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Register();
            _admin.Initialise();

            Assert.Null(_admin.GetEntity<TestInterface>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_namespace_contraint__only_matching_entities_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Where(type => type.Namespace.EndsWith("Models"))
                .Register();
            _admin.Initialise();

            Assert.NotNull(_admin.GetEntity<Car>());
            Assert.NotNull(_admin.GetEntity<Product>());
            Assert.Null(_admin.GetEntity<Category>());
            Assert.Null(_admin.GetEntity<Entity>());
            Assert.Null(_admin.GetEntity<Order>());
            Assert.Null(_admin.GetEntity<Role>());
            Assert.Null(_admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_namespace_of_contraint__only_matching_entities_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .InNamespaceOf<Car>()
                .Register();
            _admin.Initialise();

            Assert.NotNull(_admin.GetEntity<Car>());
            Assert.NotNull(_admin.GetEntity<Product>());
            Assert.Null(_admin.GetEntity<Category>());
            Assert.Null(_admin.GetEntity<Entity>());
            Assert.Null(_admin.GetEntity<Order>());
            Assert.Null(_admin.GetEntity<Role>());
            Assert.Null(_admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities_from_assembly_with_inheritance_contraint__only_matching_entities_should_be_registered()
        {
            Admin.AssemblyEntities(_testAssembly)
                .Where(type => type.IsSubclassOf(typeof(Entity)))
                .Register();
            _admin.Initialise();

            Assert.NotNull(_admin.GetEntity<Category>());
            Assert.NotNull(_admin.GetEntity<Role>());
            Assert.Null(_admin.GetEntity<Car>());
            Assert.Null(_admin.GetEntity<Product>());
            Assert.Null(_admin.GetEntity<Entity>());
            Assert.Null(_admin.GetEntity<Order>());
            Assert.Null(_admin.GetEntity<User>());
        }

        [Fact]
        public void when_registering_entities__exclude_entity_configurators()
        {
            Admin.AssemblyEntities(_testAssembly)
                .InNamespace("Ilaro.Admin.Sample.Configurators")
                .Register();
            _admin.Initialise();

            Assert.Null(_admin.GetEntity<CategoryConfigurator>());
        }
    }
}
