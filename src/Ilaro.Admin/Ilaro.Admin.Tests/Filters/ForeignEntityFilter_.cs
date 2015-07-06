using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Filters;
using Xunit;
using System;

namespace Ilaro.Admin.Tests.Filters
{
    public class ForeignEntityFilter_
    {
        private ForeignEntityFilter _filter;
        private Property _property;

        public ForeignEntityFilter_()
        {
            var entity = new Entity(typeof(TestEntity));
            _property = entity["Child"];

            _filter = new ForeignEntityFilter(_property, "1");
        }

        [Fact]
        public void generated_options_should_match()
        {
            var options = _filter.Options;
            Assert.Null(_filter.Options);
        }

        [Fact]
        public void sql_condition_should_match()
        {
            var args = new List<object>();
            var sql = _filter.GetSqlCondition("t0", ref args);

            Assert.Equal("t0[ChildId] = @0", sql);
        }

        [Fact]
        public void arguments_should_match()
        {
            var args = new List<object>();
            _filter.GetSqlCondition("t0", ref args);

            Assert.Equal(1, args.Count);
            Assert.Equal("1", args[0]);
        }
    }
}
