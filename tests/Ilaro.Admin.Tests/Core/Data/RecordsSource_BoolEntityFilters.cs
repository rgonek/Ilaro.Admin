using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;
using Ilaro.Admin.Configuration;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_BoolEntityFilters : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;
        private Property _property;

        public RecordsSource_BoolEntityFilters()
        {
            DB.Products.Insert(ProductName: "Product", Discontinued: true);
            DB.Products.Insert(ProductName: "Product2", Discontinued: false);

            _source = new RecordsSource(_admin, new Notificator());
            Entity<Product>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);
            _entity = _admin.GetEntity("Product");
            _property = _entity["Discontinued"];
        }

        [Fact]
        public void empty_bool_filter_should_return_two_records()
        {
            var result = _source.GetRecords(_entity);
            Assert.Equal(2, result.Records.Count);

            var filters = new List<BaseFilter>();
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(2, result.Records.Count);

            filters = new List<BaseFilter>
            {
                new BoolEntityFilter(_property)
            };
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(2, result.Records.Count);
        }

        [Fact]
        public void true_bool_filter_should_return_one_records()
        {
            var filters = new List<BaseFilter>
            {
                new BoolEntityFilter(_property, "1")
            };
            var result = _source.GetRecords(_entity, filters);
            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Product", result.Records[0].Values[1].AsString);
        }

        [Fact]
        public void false_bool_filter_should_return_one_records()
        {
            var filters = new List<BaseFilter>
            {
                new BoolEntityFilter(_property, "0")
            };
            var result = _source.GetRecords(_entity, filters);
            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Product2", result.Records[0].Values[1].AsString);
        }
    }
}
