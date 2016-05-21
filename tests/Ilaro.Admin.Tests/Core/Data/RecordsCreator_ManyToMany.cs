using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsCreator_ManyToMany : ManyToMany
    {
        private readonly IFetchingRecords _source;
        private readonly ICreatingRecords _creator;
        private readonly IProvidingUser _user;

        public RecordsCreator_ManyToMany()
        {
            _source = new RecordsSource(_admin, new Notificator());
            _user = A.Fake<IProvidingUser>();
            A.CallTo(() => _user.CurrentUserName()).Returns("Test");
            var executor = new DbCommandExecutor(_admin, _user);
            _creator = new RecordsCreator(_admin, executor, _user);
        }

        [Fact]
        public void when_added_value_to_many_to_many_property__many_to_many_has_one_record()
        {
            var employeeEntity = _admin.GetEntity<Employee>();
            var employeeTerritoryEntity = _admin.GetEntity<EmployeeTerritory>();

            DB.Regions.Insert(RegionID: 1, RegionDescription: "Test");
            DB.Territories.Insert(TerritoryID: 1, TerritoryDescription: "Test", RegionID: 1);

            var data = new Dictionary<string, object>();
            data["FirstName"] = "Test";
            data["LastName"] = "Test";
            var record = EntityRecordCreator.CreateRecord(employeeEntity, data);
            record.Values.FirstOrDefault(x => x.Property.Name == "Photo").DataBehavior = DataBehavior.Skip;
            var manyToManyPropertyValue = record.Values.FirstOrDefault(x => x.Property.ForeignEntity == employeeTerritoryEntity);
            manyToManyPropertyValue.Values.Add(1);

            _creator.Create(record);

            var employeeTerritoryCount = DB.EmployeeTerritories.GetCount();
            Assert.Equal(1, employeeTerritoryCount);
        }
    }
}
