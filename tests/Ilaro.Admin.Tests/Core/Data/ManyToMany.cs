using Ilaro.Admin.Tests.TestModels.Northwind;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class ManyToMany : SqlServerDatabaseTest
    {
        public ManyToMany()
        {
            setup_model();
        }

        private void setup_model()
        {
            _admin.RegisterEntity<Employee>()
                            .Property(x => x.EmployeeTerritories, x => x.ManyToMany())
                            .Property(x => x.Manager, x => x.ForeignKey("ReportsTo"));
            _admin.RegisterEntity<Region>();
            _admin.RegisterEntity<Territory>().Property(x => x.Region, x => x.ForeignKey("RegionID"));
            _admin.RegisterEntity<EmployeeTerritory>()
                .Table("EmployeeTerritories")
                .Property(x => x.EmployeeID, x => x.Id().ForeignKey("Employee"))
                .Property(x => x.Territory, x => x.Id().ForeignKey("TerritoryID"));
            _admin.Initialise();
        }

        protected int new_territory()
        {
            var newId = DB.Territories.All()
                .Select(DB.Territories.TerritoryID.Max())
                .ToScalarOrDefault<int>() + 1;

            DB.Territories.Insert(
                TerritoryID: newId,
                TerritoryDescription: "Test",
                RegionID: new_region());

            return newId;
        }

        protected int new_region()
        {
            var newId = DB.Regions.All()
                .Select(DB.Regions.RegionID.Max())
                .ToScalarOrDefault<int>() + 1;

            DB.Regions.Insert(
                RegionID: newId,
                RegionDescription: "Test");

            return newId;
        }
    }
}
