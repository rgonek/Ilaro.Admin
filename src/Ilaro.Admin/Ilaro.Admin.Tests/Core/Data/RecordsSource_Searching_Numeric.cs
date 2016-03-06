using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Fluent;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_Searching_Numeric : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;

        public RecordsSource_Searching_Numeric()
        {
            SetFakeResolver();

            DB.Products.Insert(ProductName: "Test", UnitPrice: 3);
            DB.Products.Insert(ProductName: "Product", UnitPrice: 4.23);

            _source = new RecordsSource(new Notificator());
            Entity<Product>.Register().SetSearchProperties(x => x.UnitPrice);
            Admin.Initialise(ConnectionStringName);
            _entity = Admin.GetEntity("Product");
        }

        [Fact]
        public void empty_search_gives_two_records()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: "");

            Assert.Equal(2, result.Records.Count);
        }

        [Fact]
        public void exact_number_search_gives_one_record()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: "3");

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Test", result.Records[0].Values[1].AsString);
        }

        [Fact]
        public void exact_floating_point_with_comma_number_search_gives_one_record()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: "4,23");

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Product", result.Records[0].Values[1].AsString);
        }

        [Fact]
        public void exact_floating_point_with_dot_number_search_gives_one_record()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: "4.23");

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Product", result.Records[0].Values[1].AsString);
        }

        [Fact]
        public void less_than_number_search_gives_one_record()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: "<3");

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Test", result.Records[0].Values[1].AsString);
        }

        [Fact]
        public void greater_than_number_search_gives_one_record()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: ">4.23");

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Product", result.Records[0].Values[1].AsString);
        }

        [Fact]
        public void greater_than_number_search_gives_two_records()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: ">3");

            Assert.Equal(2, result.Records.Count);
        }
    }
}
