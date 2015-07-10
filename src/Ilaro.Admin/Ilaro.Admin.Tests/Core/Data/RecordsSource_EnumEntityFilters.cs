using System;
using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_EnumEntityFilters : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;
        private Property _property;

        public RecordsSource_EnumEntityFilters()
        {
            DB.EntityChanges.Insert(new
            {
                EntityName = "Entity",
                EntityKey = "1",
                ChangedOn = DateTime.Now,
                ChangeType = EntityChangeType.Insert
            });
            DB.EntityChanges.Insert(new
            {
                EntityName = "Entity",
                EntityKey = "1",
                ChangedOn = DateTime.Now,
                ChangeType = EntityChangeType.Update
            });
            DB.EntityChanges.Insert(new
            {
                EntityName = "Entity",
                EntityKey = "1",
                ChangedOn = DateTime.Now,
                ChangeType = EntityChangeType.Delete
            });

            _source = new RecordsSource(new Notificator());
            Admin.AddEntity<EntityChange>();
            Admin.SetForeignKeysReferences();
            Admin.ConnectionStringName = ConnectionStringName;
            _entity = Admin.ChangeEntity;
            _property = _entity["ChangeType"];
        }

        [Fact]
        public void empty_enum_filter_should_return_three_records()
        {
            var result = _source.GetRecords(_entity);
            Assert.Equal(3, result.Records.Count);

            var filters = new List<IEntityFilter>();
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(3, result.Records.Count);

            filters = new List<IEntityFilter>
            {
                new EnumEntityFilter()
            };
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(3, result.Records.Count);

            var filter = new EnumEntityFilter();
            filter.Initialize(_property);
            filters = new List<IEntityFilter>
            {
                filter
            };
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(3, result.Records.Count);
        }

        [Fact]
        public void every_value_of_enum_filter_should_return_one_records()
        {
            foreach (Enum item in Enum.GetValues(typeof(EntityChangeType)))
            {
                var enumValue = Convert.ToInt32(item).ToString();
                var filter = new BoolEntityFilter();
                filter.Initialize(_property, enumValue);
                var filters = new List<IEntityFilter>
                {
                    filter
                };
                var result = _source.GetRecords(_entity, filters);
                Assert.Equal(1, result.Records.Count);
                Assert.Equal("Entity", result.Records[0].Values[1].AsString);
                Assert.Equal(enumValue, result.Records[0].Values[3].AsString);
            }
        }
    }
}
