using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_GetRecord : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;

        public RecordsSource_GetRecord()
        {
            _source = new RecordsSource(_admin, new Notificator());
            _admin.RegisterEntity<Product>();
            _admin.Initialise(ConnectionStringName);
            _entity = _admin.GetEntity("Product");
        }

        [Fact]
        public void get_record_with_correct_key_value_return_record()
        {
            DB.Products.Insert(ProductName: "Product");
            var productId = DB.Products.Insert(ProductName: "Product2").ProductID;

            var record = _source.GetRecord(_entity, productId);
            Assert.NotNull(record);
            Assert.Equal("Product2", record["ProductName"]);
        }

        [Fact]
        public void get_record_with_incorrect_key_value_return_null_object()
        {
            var record = _source.GetRecord(_entity, 0);
            Assert.Null(record);
        }
    }
}
