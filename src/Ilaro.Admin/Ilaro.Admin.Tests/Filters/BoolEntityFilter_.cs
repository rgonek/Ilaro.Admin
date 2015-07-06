using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Filters;
using Xunit;
using System;

namespace Ilaro.Admin.Tests.Filters
{
    public class BoolEntityFilter_
    {
        private readonly BoolEntityFilter _filter;

        public BoolEntityFilter_()
        {
            var entity = new Entity(typeof(TestEntity));
            var property = entity["IsSpecial"];

            _filter = new BoolEntityFilter();
            _filter.Initialize(property, "1");
        }

        [Fact]
        public void generated_options_should_match()
        {
            var options = _filter.Options.ToList();
            Assert.Equal(3, options.Count);
            Assert.Equal(String.Empty, options[0].Value);
            Assert.Equal("1", options[1].Value);
            Assert.Equal("0", options[2].Value);
        }

        [Fact]
        public void sql_condition_should_match()
        {
            var args = new List<object>();
            var sql = _filter.GetSqlCondition("t0", ref args);

            Assert.Equal("t0[IsSpecial] = @0", sql);
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
