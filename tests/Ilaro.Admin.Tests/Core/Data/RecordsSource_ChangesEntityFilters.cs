using System;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;
using Ilaro.Admin.Configuration;

namespace Ilaro.Admin.Tests.Core.Data
{
    public class RecordsSource_ChangesEntityFilters : SqlServerDatabaseTest
    {
        private readonly IFetchingRecords _source;
        private Entity _entity;
        private Property _property;

        public RecordsSource_ChangesEntityFilters()
        {
            DB.EntityChanges.Insert(new
            {
                EntityName = "Product",
                EntityKey = "1",
                ChangedOn = DateTime.Now,
                ChangeType = EntityChangeType.Insert
            });
            DB.EntityChanges.Insert(new
            {
                EntityName = "Customer",
                EntityKey = "1",
                ChangedOn = DateTime.Now,
                ChangeType = EntityChangeType.Insert
            });

            _source = new RecordsSource(_admin, new Notificator());
            Entity<EntityChange>.Register().ReadAttributes();
            _admin.Initialise(ConnectionStringName);
            _entity = _admin.ChangeEntity;
            _property = _entity["EntityName"];
        }

        [Fact]
        public void empty_changes_filter_should_return_two_records()
        {
            var result = _source.GetRecords(_entity);
            Assert.Equal(2, result.Records.Count);

            var filters = new List<BaseFilter>();
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(2, result.Records.Count);

            filters = new List<BaseFilter>
            {
                new ChangeEntityFilter(_property)
            };
            result = _source.GetRecords(_entity, filters);
            Assert.Equal(2, result.Records.Count);
        }

        [Fact]
        public void changes_filter_with_value_should_return_one_records()
        {
            var filters = new List<BaseFilter>
            {
                new ChangeEntityFilter(_property, "Product")
            };
            var result = _source.GetRecords(_entity, filters);
            Assert.Equal(1, result.Records.Count);
            Assert.Equal("Product", result.Records[0].Values[1].AsString);
        }
    }
}
