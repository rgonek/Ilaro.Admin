using FakeItEasy;
using Ilaro.Admin.Tests.TestModels.Northwind;
using System.Web.Mvc;
using Xunit;

namespace Ilaro.Admin.Tests.Core
{
    public class Entity_Initialise : TestBase
    {
        private readonly IIlaroAdmin _admin;

        public Entity_Initialise()
        {
            _admin = new IlaroAdmin();
            var resolver = A.Fake<IDependencyResolver>();
            DependencyResolver.SetResolver(resolver);
            A.CallTo(() => resolver.GetService(typeof(IIlaroAdmin)))
                .Returns(_admin);
        }

        [Fact]
        public void default_table_name_should_be_created()
        {
            var entity = _admin.RegisterEntity<EntityChange>();
            _admin.Initialise();

            Assert.Equal("[EntityChanges]", entity.TableName);
        }
    }
}
