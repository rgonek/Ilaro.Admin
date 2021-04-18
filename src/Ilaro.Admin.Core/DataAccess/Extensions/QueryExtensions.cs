using SqlKata;
using System.Data;
using SqlKata.Execution;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.DataAccess.Extensions
{
    public static class QueryExtensions
    {
        public static Query OrderBy(this Query query, OrderDirection orderDirection, params string[] columns)
            => orderDirection == OrderDirection.Asc ? query.OrderBy(columns) : query.OrderByDesc(columns);

        public static Query OrderBy(this Query query, string column, OrderDirection orderDirection)
            => query.OrderBy(orderDirection, column);

        public static int Update(this Query query, string column, object value, IDbTransaction transaction = null, int? timeout = null)
            => query.Update(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(column, value) }, transaction, timeout);
    }
}
