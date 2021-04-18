using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.DataAccess.Extensions;
using SqlKata.Execution;
using Dawn;

namespace Ilaro.Admin.Core.DataAccess
{
    public class RecordUpdater : IRecordUpdater
    {
        private readonly QueryFactory _db;
        private readonly IRecordFetcher _source;
        private readonly IUser _user;

        public RecordUpdater(
            QueryFactory db,
            IRecordFetcher source,
            IUser user)
        {
            Guard.Argument(db, nameof(db)).NotNull();
            Guard.Argument(source, nameof(source)).NotNull();
            Guard.Argument(user, nameof(user)).NotNull();

            _db = db;
            _source = source;
            _user = user;
        }

        public bool Update(
            EntityRecord entityRecord,
            object concurrencyCheckValue = null)
        {
            _db.Transactionally(
                Update(entityRecord),
                ConcurrencyCheck(entityRecord, concurrencyCheckValue),
                UpdateOneToMany(entityRecord),
                UpdateManyToMany(entityRecord));

            return true;
        }

        private IEnumerable<Action<IDbTransaction>> Update(EntityRecord entityRecord)
        {
            var updateProperties = entityRecord.Values
                .WhereIsNotSkipped()
                .WhereNotOneToMany()
                .Where(value => value.Property.IsCreatable)
                .Where(value => value.Property.IsKey == false)
                .DistinctBy(x => x.Property.Column)
                .ToKeyValuePairCollection(_user);
            return new Action<IDbTransaction>[]
            {
                tx=>
                {
                    var query = _db.Query(entityRecord.Entity.Table);
                    foreach (var key in entityRecord.Id)
                    {
                        query.Where(key.Property.Column, key.AsObject);
                    }
                    query.Update(updateProperties, tx);
                }
            };
        }

        private IEnumerable<Action<IDbTransaction>> ConcurrencyCheck(EntityRecord entityRecord, object concurrencyCheckValue)
        {
            if (entityRecord.Entity.ConcurrencyCheckEnabled == false)
            {
                return Enumerable.Empty<Action<IDbTransaction>>();
            }
            Guard.Argument(concurrencyCheckValue, nameof(concurrencyCheckValue)).NotNull();

            return new Action<IDbTransaction>[]
            {
                tx =>
                {

                }
            };
        }

        private IEnumerable<Action<IDbTransaction>> UpdateOneToMany(EntityRecord entityRecord)
        {
            if (entityRecord.Id.IsComposite)
            {
                return Enumerable.Empty<Action<IDbTransaction>>();
            }

            var actions = new List<Action<IDbTransaction>>();
            //foreach (var propertyValue in entityRecord.Values.WhereOneToMany())
            //{
            //    actions.Add(tx =>
            //    {
            //    });
            //}

            return actions;
        }

        private IEnumerable<Action<IDbTransaction>> UpdateManyToMany(EntityRecord entityRecord)
        {
            if (entityRecord.Id.IsComposite)
            {
                return Enumerable.Empty<Action<IDbTransaction>>();
            }

            var actions = new List<Action<IDbTransaction>>();
            //foreach (var propertyValue in entityRecord.Values.WhereManyToMany())
            //{
            //    actions.Add(tx =>
            //    {

            //    });
            //}

            return actions;
        }
    }
}