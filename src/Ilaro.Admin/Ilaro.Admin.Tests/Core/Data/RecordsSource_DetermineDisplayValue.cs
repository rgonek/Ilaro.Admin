using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_DetermineDisplayValue : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;

        public RecordsSource_DetermineDisplayValue()
        {
            _source = new RecordsSource(_admin, new Notificator());
            Entity<Product>.Register().ReadAttributes();
            Entity<Customer>.Register().ReadAttributes();
            Entity<EmployeeTerritory>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);
        }

        [Fact]
        public void determine_value_using_to_string_method()
        {
            DB.Products.Insert(ProductName: "Product");

            var productEntity = _admin.GetEntity("Product");

            var result = _source.GetRecords(productEntity, determineDisplayValue: true);

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Product_ToString", result.Records[0].DisplayName);
        }

        [Fact]
        public void determine_value_using_property()
        {
            DB.Customers.Insert(CustomerID: "MICRO", CompanyName: "Microsoft");

            var customerEntity = _admin.GetEntity("Customer");

            var result = _source.GetRecords(customerEntity, determineDisplayValue: true);

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Microsoft", result.Records[0].DisplayName);
        }

        [Fact]
        public void determine_value_using_id()
        {
            var employee = DB.Employees.Insert(LastName: "Last", FirstName: "First");
            DB.Regions.Insert(RegionID: 1, RegionDescription: "Test");
            DB.Territories.Insert(TerritoryID: 0123, TerritoryDescription: "Test", RegionID: 1);
            var employeeTorritory = DB.EmployeeTerritories.Insert(TerritoryID: 0123, EmployeeID: employee.EmployeeID);

            var employeeTerritoryEntity = _admin.GetEntity("EmployeeTerritory");

            var result = _source.GetRecords(employeeTerritoryEntity, determineDisplayValue: true);

            Assert.Equal(1, result.Records.Count);
            Assert.Equal("#" + employeeTorritory.EmployeeTerritoryID, result.Records[0].DisplayName);
        }
    }
}
