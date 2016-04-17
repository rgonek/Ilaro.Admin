using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_Paging : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;

        public RecordsSource_Paging()
        {
            DB.Products.Insert(ProductName: "Product");
            DB.Products.Insert(ProductName: "Product2");
            DB.Products.Insert(ProductName: "Product3");

            _source = new RecordsSource(_admin, new Notificator());
            Entity<Product>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);
            _entity = _admin.GetEntity("Product");
        }

        [Fact]
        public void doesnt_returns_paged_results_when_page_and_take_not_specified()
        {
            var result = _source.GetRecords(_entity);

            Assert.Equal(3, result.Records.Count);
            Assert.Equal(0, result.TotalItems);
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public void doesnt_returns_paged_results_when_page_specified_and_take_not_specified()
        {
            var result = _source.GetRecords(_entity, page: 1);

            Assert.Equal(3, result.Records.Count);
            Assert.Equal(0, result.TotalItems);
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public void doesnt_returns_paged_results_when_page_not_specified_and_take_specified()
        {
            var result = _source.GetRecords(_entity, take: 1);

            Assert.Equal(3, result.Records.Count);
            Assert.Equal(0, result.TotalItems);
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public void returns_paged_results_when_page_and_take_specified()
        {
            var result = _source.GetRecords(_entity, page: 2, take: 2);

            Assert.Equal(1, result.Records.Count);
            Assert.Equal(3, result.TotalItems);
            Assert.Equal(2, result.TotalPages);
        }
    }
}
