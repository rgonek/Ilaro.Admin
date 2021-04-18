using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dawn;
using Ilaro.Admin.Core.DataAccess.Extensions;
using Ilaro.Admin.Core.Extensions;
using SqlKata.Execution;

namespace Ilaro.Admin.Core.DataAccess
{
    public class RecordCreator : IRecordCreator
    {
        private readonly QueryFactory _db;
        private readonly IUser _user;

        public RecordCreator(QueryFactory db, IUser user)
        {
            Guard.Argument(db, nameof(db)).NotNull();
            Guard.Argument(user, nameof(user)).NotNull();

            _db = db;
            _user = user;
        }

        public IdValue Create(EntityRecord entityRecord)
            => _db.InsertTransactionally(
                tx => Insert(entityRecord, tx),
                UpdateOneToMany(entityRecord).Union(
                UpdateManyToMany(entityRecord)));

        private IdValue Insert(EntityRecord entityRecord, IDbTransaction tx)
        {
            var propertyValues = entityRecord.Values
                .WhereIsNotSkipped()
                .WhereIsNotOneToMany()
                .DistinctBy(x => x.Property.Column)
                .Where(x => x.Property.IsAutoKey == false)
                .ToKeyValuePairCollection(_user);

            var idResult = _db.Query(entityRecord.Entity.Table)
                .InsertGetId<object>(propertyValues, tx);
            //var idType = "int";
            //var insertedId = "SCOPE_IDENTITY()";
            //if (entityRecord.Id.Count > 1 || entityRecord.Id.First().Property.TypeInfo.IsString)
            //{
            //    idType = "nvarchar(max)";
            //    insertedId = "@" + counter;
            //    cmd.AddParam(entityRecord.Id);
            //}

            return entityRecord.Entity.Id.Fill(idResult);
        }

        private IEnumerable<Action<IdValue, IDbTransaction>> UpdateOneToMany(EntityRecord entityRecord)
        {
            if (entityRecord.Id.IsComposite)
            {
                return Enumerable.Empty<Action<IdValue, IDbTransaction>>();
            }

            var actions = new List<Action<IdValue, IDbTransaction>>();
            foreach (var propertyValue in entityRecord.Values
                .WhereOneToMany()
                .Where(x => x.Property.IsManyToMany == false)
                .Where(value => value.Values.IsNullOrEmpty() == false))
            {
                var values = propertyValue.Values
                    .Select(x => x.ToStringSafe().Split(Id.ColumnSeparator).Select(y => y.Trim()).ToList())
                    .ToList();
                actions.Add((newId, tx) =>
                {
                    var query = _db.Query(propertyValue.Property.ForeignEntity.Table);
                    for (int i = 0; i < propertyValue.Property.ForeignEntity.Id.Count; i++)
                    {
                        var key = propertyValue.Property.ForeignEntity.Id[i];
                        query.WhereIn(key.Column, values[i]);
                    }
                    query.Update(entityRecord.Entity.Id.First().Column, newId, tx);

                });
            }

            return actions;
        }

        private IEnumerable<Action<IdValue, IDbTransaction>> UpdateManyToMany(EntityRecord entityRecord)
        {
            if (entityRecord.Id.IsComposite)
            {
                return Enumerable.Empty<Action<IdValue, IDbTransaction>>();
            }

            var actions = new List<Action<IdValue, IDbTransaction>>();
            foreach (var propertyValue in entityRecord.Values
                .WhereOneToMany()
                .Where(x => x.Property.IsManyToMany))
            {
                var selectedValues = propertyValue.Values.Select(x => x.ToStringSafe()).ToList();

                var idsToAdd = selectedValues
                    .ToList();
                if (idsToAdd.Any() == false)
                {
                    continue;
                }
                var mtmEntity = GetEntityToLoad(propertyValue.Property);
                foreach (var idToAdd in idsToAdd)
                {
                    var foreignEntity = propertyValue.Property.ForeignEntity;
                    var key1 =
                        foreignEntity.ForeignKeys.FirstOrDefault(
                            x => x.ForeignEntity == propertyValue.Property.Entity);
                    var key2 =
                        foreignEntity.ForeignKeys.FirstOrDefault(
                            x => x.ForeignEntity == mtmEntity);

                    actions.Add((newId, tx) =>
                    {
                        var columns = new[] { key1.Column, key2.Column };
                        var values = new[] { newId.First().AsObject, idToAdd };
                        _db.Query(foreignEntity.Table)
                            .Insert(columns, new[] { values }, tx);

                    });
                }
            }

            return actions;
        }

        private static Entity GetEntityToLoad(Property foreignProperty)
        {
            if (foreignProperty.IsManyToMany)
            {
                return foreignProperty.ForeignEntity.ForeignKeys
                    .First(x => x.ForeignEntity != foreignProperty.Entity)
                    .ForeignEntity;
            }

            return foreignProperty.ForeignEntity;
        }
    }
}