using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_Searching_String : SqlServerDatabaseTest
    {
        private readonly IIlaroAdmin _admin;
        private readonly IFetchingRecords _source;
        private Entity _entity;

        public RecordsSource_Searching_String()
        {
            _admin = new IlaroAdmin();

            DB.Products.Insert(ProductName: "Test");
            DB.Products.Insert(ProductName: "Product");

            _source = new RecordsSource(_admin, new Notificator());
            _admin.RegisterEntity<Product>();
            _admin.Initialise(ConnectionStringName);
            _entity = _admin.GetEntity("Product");
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
        public void specified_search_gives_one_record()
        {
            var result = _source.GetRecords(
                _entity,
                searchQuery: "Te");

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Test", result.Records[0].Values[1].AsString);
        }
    }
}
