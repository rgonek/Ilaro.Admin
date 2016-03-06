using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_Ordering : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;

        public RecordsSource_Ordering()
        {
            SetFakeResolver();

            DB.Products.Insert(ProductName: "Product");
            DB.Products.Insert(ProductName: "Product2");

            _source = new RecordsSource(new Notificator());
            Admin.RegisterEntity<Product>();
            Admin.Initialise(ConnectionStringName);
            _entity = Admin.GetEntity("Product");
        }

        [Fact]
        public void ascending_ordering_gives_correct_result()
        {
            var result = _source.GetRecords(
                _entity,
                order: "ProductName",
                orderDirection: "ASC");

            Assert.Equal(2, result.Records.Count);
            Assert.Equal("Product", result.Records[0].Values[1].AsString);
            Assert.Equal("Product2", result.Records[1].Values[1].AsString);
        }

        [Fact]
        public void descending_ordering_gives_correct_result()
        {
            var result = _source.GetRecords(
                _entity,
                order: "ProductName",
                orderDirection: "DESC");

            Assert.Equal(2, result.Records.Count);
            Assert.Equal("Product2", result.Records[0].Values[1].AsString);
            Assert.Equal("Product", result.Records[1].Values[1].AsString);
        }
    }
}
