using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_GetEntityWithData : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;

        public RecordsSource_GetEntityWithData()
        {
            _source = new RecordsSource(_admin, new Notificator());
            Entity<Product>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);
        }

        [Fact]
        public void get_record_with_correct_data_return_record()
        {
            DB.Products.Insert(ProductName: "Product");
            var productId = DB.Products.Insert(ProductName: "Product2").ProductID;

            var enetity = _admin.GetEntity("Product");
            EntityRecord entityRecord = _source.GetEntityRecord(enetity, productId.ToString());
            Assert.NotNull(enetity);
            Assert.Equal("Product2", entityRecord["ProductName"].AsString);
        }

        [Fact]
        public void get_entity_with_data_with_incorrect_key_value_return_null_object()
        {
            var enetity = _admin.GetEntity("Product");
            var entityRecord = _source.GetEntityRecord(enetity, "0");
            Assert.Null(entityRecord);
        }
    }
}
