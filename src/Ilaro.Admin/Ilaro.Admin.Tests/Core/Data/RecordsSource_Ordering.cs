using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_Ordering : SqlServerDatabaseTest
    {
        private readonly IIlaroAdmin _admin;
        private readonly IFetchingRecords _source;
        private Entity _entity;

        public RecordsSource_Ordering()
        {
            _admin = new IlaroAdmin();

            DB.Products.Insert(ProductName: "Product");
            DB.Products.Insert(ProductName: "Product2");

            _source = new RecordsSource(_admin, new Notificator());
            _admin.RegisterEntity<Product>();
            _admin.Initialise(ConnectionStringName);
            _entity = _admin.GetEntity("Product");
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
