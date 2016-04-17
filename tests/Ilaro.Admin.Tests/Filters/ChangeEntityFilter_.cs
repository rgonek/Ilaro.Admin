using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Filters;
using Xunit;

namespace Ilaro.Admin.Tests.Filters
{
    public class ChangeEntityFilter_
    {
        private ChangeEntityFilter _filter;
        private Property _property;

        public ChangeEntityFilter_()
        {
            var entity = new Entity(typeof(TestEntity));
            _property = entity["Name"];

            _filter = new ChangeEntityFilter(_property, "entity_name");
        }

        [Fact]
        public void generated_options_should_be_empty()
        {
            Assert.Equal(0, _filter.Options.Count);
        }

        [Fact]
        public void sql_condition_should_match()
        {
            var args = new List<object>();
            var sql = _filter.GetSqlCondition("t0", ref args);

            Assert.Equal("t0[Name] = @0", sql);
        }

        [Fact]
        public void arguments_should_match()
        {
            var args = new List<object>();
            _filter.GetSqlCondition("t0", ref args);

            Assert.Equal(1, args.Count);
            Assert.Equal("entity_name", args[0]);
        }
    }
}
