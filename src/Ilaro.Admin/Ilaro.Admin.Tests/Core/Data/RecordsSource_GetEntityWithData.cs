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
            _source = new RecordsSource(new Notificator());
            AdminInitialise.AddEntity<Product>();
            AdminInitialise.SetForeignKeysReferences();
            AdminInitialise.ConnectionStringName = ConnectionStringName;
        }

        [Fact]
        public void get_record_with_correct_data_return_record()
        {
            DB.Products.Insert(ProductName: "Product");
            var productId = DB.Products.Insert(ProductName: "Product2").ProductID;

            var record = _source.GetEntityWithData("Product", productId.ToString()) as Entity;
            Assert.NotNull(record);
            Assert.Equal("Product2", record["ProductName"].Value.AsString);
        }

        [Fact]
        public void get_entity_with_data_with_incorrect_entity_name_return_null_object()
        {
            var record = _source.GetEntityWithData("Produc", "1");
            Assert.Null(record);
        }

        [Fact]
        public void get_entity_with_data_with_incorrect_key_value_return_null_object()
        {
            var record = _source.GetEntityWithData("Product", "0");
            Assert.Null(record);
        }
    }
}
