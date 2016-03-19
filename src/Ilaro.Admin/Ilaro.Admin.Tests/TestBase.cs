using FakeItEasy;
using System.Web.Mvc;

namespace Ilaro.Admin.Tests
{
    public class TestBase
    {
        protected readonly IIlaroAdmin _admin;

        public TestBase()
        {
            _admin = new IlaroAdmin();
            var resolver = A.Fake<IDependencyResolver>();
            DependencyResolver.SetResolver(resolver);
            A.CallTo(() => resolver.GetService(typeof(IIlaroAdmin)))
                .Returns(_admin);
        }
    }
}
