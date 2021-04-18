using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ilaro.Admin.Core.DataAccess.Extensions
{
    public static class QueryFactoryExtensions
    {
        public static void Transactionally(this QueryFactory db, params Action<IDbTransaction>[] actions)
        {
            if (db.Connection.State == ConnectionState.Closed)
            {
                db.Connection.Open();
            }
            using (var tx = db.Connection.BeginTransaction())
            {
                try
                {
                    foreach (var action in actions)
                    {
                        action(tx);
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                }
            }
        }

        public static IdValue InsertTransactionally(
            this QueryFactory db,
            Func<IDbTransaction, IdValue> insert,
            params Action<IdValue, IDbTransaction>[] postInsertActions)
        {
            if (db.Connection.State == ConnectionState.Closed)
            {
                db.Connection.Open();
            }
            using (var tx = db.Connection.BeginTransaction())
            {
                try
                {
                    var newId = insert(tx);
                    foreach (var action in postInsertActions)
                    {
                        action(newId, tx);
                    }

                    tx.Commit();

                    return newId;
                }
                catch
                {
                    tx.Rollback();

                    return null;
                }
            }
        }

        public static IdValue InsertTransactionally(
            this QueryFactory db,
            Func<IDbTransaction, IdValue> insert,
            IEnumerable<Action<IdValue, IDbTransaction>> postInsertActions)
            => db.InsertTransactionally(insert, postInsertActions.ToArray());
    }
}
