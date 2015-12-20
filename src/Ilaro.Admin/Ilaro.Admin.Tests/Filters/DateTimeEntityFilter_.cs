using System;
using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Filters;
using Xunit;

namespace Ilaro.Admin.Tests.Filters
{
    public class DateTimeEntityFilter_
    {
        private DateTimeEntityFilter _filter;
        private IKnowTheTime _fakeClock;
        private Property _property;

        public DateTimeEntityFilter_()
        {
            var entity = new Entity(typeof(TestEntity));
            _property = entity["DateAdd"];

            _fakeClock = new FakeClock(
                () => new DateTime(2015, 7, 20),
                () => new DateTime(2015, 7, 20));
            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.20");
        }

        [Fact]
        public void selected_value_should_match()
        {
            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.20");
            var options = _filter.Options.ToList();
            Assert.True(options[1].Selected);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.19");
            options = _filter.Options.ToList();
            Assert.True(options[2].Selected);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.13-2015.07.20");
            options = _filter.Options.ToList();
            Assert.True(options[3].Selected);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.06.20-2015.07.20");
            options = _filter.Options.ToList();
            Assert.True(options[4].Selected);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.04.20-2015.07.20");
            options = _filter.Options.ToList();
            Assert.True(options[5].Selected);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.01.20-2015.07.20");
            options = _filter.Options.ToList();
            Assert.True(options[6].Selected);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2014.07.20-2015.07.20");
            options = _filter.Options.ToList();
            Assert.True(options[7].Selected);
        }

        [Fact]
        public void generated_options_should_match()
        {
            var options = _filter.Options.ToList();
            Assert.Equal(9, options.Count);
            Assert.Equal(String.Empty, options[0].Value);
            Assert.Equal("2015.07.20", options[1].Value);
            Assert.Equal("2015.07.19", options[2].Value);
            Assert.Equal("2015.07.13-2015.07.20", options[3].Value);
            Assert.Equal("2015.06.20-2015.07.20", options[4].Value);
            Assert.Equal("2015.04.20-2015.07.20", options[5].Value);
            Assert.Equal("2015.01.20-2015.07.20", options[6].Value);
            Assert.Equal("2014.07.20-2015.07.20", options[7].Value);
        }

        [Fact]
        public void sql_condition_should_match()
        {
            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.20");
            var args = new List<object>();
            var sql = _filter.GetSqlCondition("t0", ref args);
            Assert.Equal("(t0[DateAdd] >= @0 AND t0[DateAdd] <= @1)", sql);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.13-2015.07.20");
            args = new List<object>();
            sql = _filter.GetSqlCondition("t0", ref args);
            Assert.Equal("(t0[DateAdd] >= @0 AND t0[DateAdd] <= @1)", sql);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "-2015.07.20");
            args = new List<object>();
            sql = _filter.GetSqlCondition("t0", ref args);
            Assert.Equal("t0[DateAdd] <= @0", sql);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.13-");
            args = new List<object>();
            sql = _filter.GetSqlCondition("t0", ref args);
            Assert.Equal("t0[DateAdd] >= @0", sql);
        }

        [Fact]
        public void arguments_should_match()
        {
            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.20");
            var args = new List<object>();
            _filter.GetSqlCondition("t0", ref args);
            Assert.Equal(2, args.Count);
            Assert.Equal("2015.07.20 00:00", args[0]);
            Assert.Equal("2015.07.20 23:59", args[1]);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.13-2015.07.20");
            args = new List<object>();
            _filter.GetSqlCondition("t0", ref args);
            Assert.Equal(2, args.Count);
            Assert.Equal("2015.07.13 00:00", args[0]);
            Assert.Equal("2015.07.20 23:59", args[1]);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "-2015.07.20");
            args = new List<object>();
            _filter.GetSqlCondition("t0", ref args);
            Assert.Equal(1, args.Count);
            Assert.Equal("2015.07.20 23:59", args[0]);

            _filter = new DateTimeEntityFilter(_fakeClock, _property, "2015.07.13-");
            args = new List<object>();
            _filter.GetSqlCondition("t0", ref args);
            Assert.Equal(1, args.Count);
            Assert.Equal("2015.07.13 00:00", args[0]);
        }
    }
}
