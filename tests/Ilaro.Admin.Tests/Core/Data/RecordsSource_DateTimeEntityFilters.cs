using System;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;
using Ilaro.Admin.Configuration;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_DateTimeEntityFilters : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;
        private Property _property;
        private IKnowTheTime _fakeClock;

        public RecordsSource_DateTimeEntityFilters()
        {
            _fakeClock = new FakeClock(
                () => new DateTime(2015, 7, 20),
                () => new DateTime(2015, 7, 20));

            DB.Orders.Insert(ShipCity: "City1", OrderDate: "2015.07.20 11:33");
            DB.Orders.Insert(ShipCity: "City2", OrderDate: "2015.07.19 11:33");
            DB.Orders.Insert(ShipCity: "City3", OrderDate: "2015.06.20 11:33");
            DB.Orders.Insert(ShipCity: "City4", OrderDate: "2014.08.20 11:33");

            _source = new RecordsSource(_admin, new Notificator());
            Entity<Order>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);
            _entity = _admin.GetEntity("Order");
            _property = _entity["OrderDate"];
        }

        [Fact]
        public void empty_date_time_filter_should_return_four_records()
        {
            var result = _source.GetRecords(_entity);
            Assert.Equal(4, result.Records.Count);

            var filters = new List<BaseFilter>();
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(4, result.Records.Count);

            filters = new List<BaseFilter>
            {
                new DateTimeEntityFilter(_fakeClock, _property)
            };
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(4, result.Records.Count);
        }

        [Theory]
        [InlineData("2015.07.20", 1)]
        [InlineData("2015.07.19", 1)]
        [InlineData("2015.07.13-2015.07.20", 2)]
        [InlineData("2015.06.20-2015.07.20", 3)]
        [InlineData("2015.04.20-2015.07.20", 3)]
        [InlineData("2015.01.20-2015.07.20", 3)]
        [InlineData("2014.07.20-2015.07.20", 4)]
        [InlineData("2014.07.20-", 4)]
        [InlineData("-2015.07.20", 4)]
        [InlineData("-2014.08.20", 1)]
        [InlineData("2015.07.20-", 1)]
        public void date_time_filter_with_provided_value__returned_count_results_should_match_with_provided_count(string value, int resultsCount)
        {
            var filters = new List<BaseFilter>
            {
                new DateTimeEntityFilter(_fakeClock,_property, value)
            };
            var result = _source.GetRecords(_entity, filters);
            Assert.Equal(resultsCount, result.Records.Count);
        }
    }
}
