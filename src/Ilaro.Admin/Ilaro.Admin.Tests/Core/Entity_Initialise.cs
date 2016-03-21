using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core
{
    public class Entity_Initialise : TestBase
    {
        [Fact]
        public void default_table_name_should_be_created()
        {
            _admin.RegisterEntity<EntityChange>();
            _admin.Initialise();
            var entity =  _admin.GetEntity<EntityChange>();

            Assert.Equal("[EntityChanges]", entity.TableName);
        }
    }
}
