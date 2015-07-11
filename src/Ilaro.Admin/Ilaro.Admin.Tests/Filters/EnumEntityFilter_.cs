using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Filters;
using Xunit;
using System;

namespace Ilaro.Admin.Tests.Filters
{
    public class EnumEntityFilter_
    {
        private EnumEntityFilter _filter;
        private Property _property;

        public EnumEntityFilter_()
        {
            var entity = new Entity(typeof(TestEntity));
            _property = entity["Option"];

            _filter = new EnumEntityFilter(_property, "0");
        }

        [Fact]
        public void generated_options_should_match()
        {
            var options = _filter.Options.ToList();
            Assert.Equal(3, options.Count);
            Assert.Equal(String.Empty, options[0].Value);
            Assert.Equal("Option 1", options[1].Text);
            Assert.Equal("0", options[1].Value);
            Assert.Equal("Option 2", options[2].Text);
            Assert.Equal("1", options[2].Value);
        }

        [Fact]
        public void selected_value_should_match()
        {
            _filter = new EnumEntityFilter(_property, "0");
            var options = _filter.Options.ToList();
            Assert.True(options[1].Selected);

            _filter = new EnumEntityFilter(_property, "1");
            options = _filter.Options.ToList();
            Assert.True(options[2].Selected);
        }

        [Fact]
        public void sql_condition_should_match()
        {
            var args = new List<object>();
            var sql = _filter.GetSqlCondition("t0", ref args);

            Assert.Equal("t0[Option] = @0", sql);
        }

        [Fact]
        public void arguments_should_match()
        {
            var args = new List<object>();
            _filter.GetSqlCondition("t0", ref args);

            Assert.Equal(1, args.Count);
            Assert.Equal("0", args[0]);
        }
    }
}
