using FakeItEasy;
using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Models;
using Ilaro.Admin.Tests.TestModels.Northwind;
using System.Collections.Generic;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsDeleter_ : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private readonly IDeletingRecords _deleter;
        private readonly IFetchingRecordsHierarchy _hierarchySource;
        private readonly IProvidingUser _user;

        public RecordsDeleter_()
        {
            _user = A.Fake<IProvidingUser>();
            A.CallTo(() => _user.CurrentUserName()).Returns("Test");
            var executor = new DbCommandExecutor(_admin, _user);
            _hierarchySource = new RecordsHierarchySource(_admin);
            _deleter = new RecordsDeleter(_admin, executor, _hierarchySource, _user);
            _source = new RecordsSource(_admin, new Notificator());
        }

        [Fact]
        public void when_record_has_foreign_record_and_delete_option_is_set_null__delete_root_record_and_set_foreign_key_to_null()
        {
            Entity<Customer>.Register().ReadAttributes();
            Entity<Order>.Register().ReadAttributes();
            Entity<OrderDetail>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);

            var customerId = "CTEST";
            DB.Customers.Insert(CustomerID: customerId, CompanyName: "test");
            var orderId = DB.Orders.Insert(CustomerID: customerId).OrderID;
            var productId = DB.Products.Insert(ProductName: "test").ProductID;
            DB.OrderDetails.Insert(OrderID: orderId, ProductID: productId);

            var customerEntity = _admin.GetEntity<Customer>();
            var entityRecord = _source.GetEntityRecord(
                customerEntity,
                customerId);

            var deleteOptions = new Dictionary<string, PropertyDeleteOption>
            {
                { "Order", new PropertyDeleteOption { DeleteOption = CascadeOption.Detach } }
            };

            var result = _deleter.Delete(entityRecord, deleteOptions);

            Assert.True(result);

            Assert.Equal(0, DB.Customers.All().ToList().Count);
            Assert.Equal(null, DB.Orders.All().FirstOrDefault().CustomerID);
            Assert.Equal(1, DB.OrderDetails.All().ToList().Count);
        }

        [Fact]
        public void when_record_has_foreign_record_and_delete_option_is_cascade_delete_for_all_properties__delete_all_related_records()
        {
            Entity<Customer>.Register().ReadAttributes();
            Entity<Order>.Register().ReadAttributes();
            Entity<OrderDetail>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);

            var customerId = "CTEST";
            DB.Customers.Insert(CustomerID: customerId, CompanyName: "test");
            var orderId = DB.Orders.Insert(CustomerID: customerId).OrderID;
            var productId = DB.Products.Insert(ProductName: "test").ProductID;
            DB.OrderDetails.Insert(OrderID: orderId, ProductID: productId);

            var customerEntity = _admin.GetEntity<Customer>();
            var entityRecord = _source.GetEntityRecord(
                customerEntity,
                customerId);

            var deleteOptions = new Dictionary<string, PropertyDeleteOption>
            {
                { "Order", new PropertyDeleteOption { DeleteOption = CascadeOption.Delete } },
                { "Order-OrderDetail", new PropertyDeleteOption { DeleteOption = CascadeOption.Delete, Level = 1 } }
            };

            var result = _deleter.Delete(entityRecord, deleteOptions);

            Assert.True(result);

            Assert.Equal(0, DB.Customers.All().ToList().Count);
            Assert.Equal(0, DB.Orders.All().ToList().Count);
            Assert.Equal(0, DB.OrderDetails.All().ToList().Count);
        }

        [Fact]
        public void when_record_has_foreign_record_and_soft_delete_is_enabled__dont_delete_foreign_records_and_update_record_with_default_delete_values()
        {
            Entity<Customer>.Register()
                .ReadAttributes()
                .SoftDelete()
                .Property(x => x.Fax, x => x.OnDelete(ValueBehavior.UtcNow));
            Entity<Order>.Register().ReadAttributes();
            Entity<OrderDetail>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);

            var customerId = "CTEST";
            DB.Customers.Insert(CustomerID: customerId, CompanyName: "test");
            var orderId = DB.Orders.Insert(CustomerID: customerId).OrderID;
            var productId = DB.Products.Insert(ProductName: "test").ProductID;
            DB.OrderDetails.Insert(OrderID: orderId, ProductID: productId);

            var customerEntity = _admin.GetEntity<Customer>();
            var entityRecord = _source.GetEntityRecord(
                customerEntity,
                customerId);

            var result = _deleter.Delete(entityRecord, null);

            Assert.True(result);

            var customer = DB.Customers.All().ToList()[0];
            Assert.NotNull(customer);
            Assert.NotNull(customer.Fax);
            Assert.Equal(1, DB.Orders.All().ToList().Count);
            Assert.Equal(1, DB.OrderDetails.All().ToList().Count);
        }
    }
}
