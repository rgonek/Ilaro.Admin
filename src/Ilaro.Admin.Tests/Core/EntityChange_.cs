using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core
{
    public class EntityChange_ : TestBase
    {
        [Fact]
        public void when_set_entity_change_as_deletable_it_is_always_not_deletable()
        {
            _admin.RegisterEntity<EntityChange>().Deletable(true);
            _admin.Initialise();

            var entityChange = _admin.ChangeEntity;

            Assert.False(entityChange.AllowDelete);
        }
        [Fact]
        public void when_set_entity_change_as_editable_it_is_always_not_editable()
        {
            _admin.RegisterEntity<EntityChange>().Editable(true);
            _admin.Initialise();

            var entityChange = _admin.ChangeEntity;

            Assert.False(entityChange.AllowEdit);
        }

        [Fact]
        public void entity_change_always_not_allowed_add()
        {
            _admin.RegisterEntity<EntityChange>();
            _admin.Initialise();

            var entityChange = _admin.ChangeEntity;

            Assert.False(entityChange.AllowAdd);
        }
    }
}
