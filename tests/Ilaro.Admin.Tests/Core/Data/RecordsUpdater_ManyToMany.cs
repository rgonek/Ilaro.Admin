using System.Linq;
using FakeItEasy;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsUpdater_ManyToMany : ManyToMany
    {
        private readonly IFetchingRecords _source;
        private readonly IUpdatingRecords _updater;
        private readonly IProvidingUser _user;

        public RecordsUpdater_ManyToMany()
        {
            _source = new RecordsSource(_admin, new Notificator());
            _user = A.Fake<IProvidingUser>();
            A.CallTo(() => _user.CurrentUserName()).Returns("Test");
            var executor = new DbCommandExecutor(_admin, _user);
            _updater = new RecordsUpdater(_admin, executor, _source, _user);
        }

        [Fact]
        public void when_removed_all_values_from_many_to_many_property__many_to_many_table_is_empty()
        {
            var employeeEntity = _admin.GetEntity<Employee>();
            var employeeTerritoryEntity = _admin.GetEntity<EmployeeTerritory>();

            var employee = DB.Employees.Insert(FirstName: "Test", LastName: "Test");
            DB.Regions.Insert(RegionID: 1, RegionDescription: "Test");
            DB.Territories.Insert(TerritoryID: 1, TerritoryDescription: "Test", RegionID: 1);
            DB.EmployeeTerritories.Insert(EmployeeID: employee.EmployeeID, TerritoryID: 1);

            var record = _source.GetEntityRecord(employeeEntity, ((int)employee.EmployeeID).ToString());
            record.Values.FirstOrDefault(x => x.Property.Name == "Photo").DataBehavior = DataBehavior.Skip;
            var manyToManyPropertyValue = record.Values.FirstOrDefault(x => x.Property.ForeignEntity == employeeTerritoryEntity);

            manyToManyPropertyValue.Values.Clear();

            _updater.Update(record);

            var employeeTerritoryCount = DB.EmployeeTerritories.GetCount();
            Assert.Equal(0, employeeTerritoryCount);
        }

        [Fact]
        public void when_added_value_to_many_to_many_property__many_to_many_has_one_record()
        {
            var employeeEntity = _admin.GetEntity<Employee>();
            var employeeTerritoryEntity = _admin.GetEntity<EmployeeTerritory>();

            var employee = DB.Employees.Insert(FirstName: "Test", LastName: "Test");
            DB.Regions.Insert(RegionID: 1, RegionDescription: "Test");
            DB.Territories.Insert(TerritoryID: 1, TerritoryDescription: "Test", RegionID: 1);

            var record = _source.GetEntityRecord(employeeEntity, ((int)employee.EmployeeID).ToString());
            record.Values.FirstOrDefault(x => x.Property.Name == "Photo").DataBehavior = DataBehavior.Skip;
            var manyToManyPropertyValue = record.Values.FirstOrDefault(x => x.Property.ForeignEntity == employeeTerritoryEntity);

            manyToManyPropertyValue.Values.Add(1);

            _updater.Update(record);

            var employeeTerritoryCount = DB.EmployeeTerritories.GetCount();
            Assert.Equal(1, employeeTerritoryCount);
        }
    }
}
