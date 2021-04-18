using SqlKata;

namespace Ilaro.Admin.Core.DataAccess.Extensions
{
    public static class QueryExtensions
    {
        public static Query OrderBy(this Query query, OrderDirection orderDirection, params string[] columns)
            => orderDirection == OrderDirection.Asc ? query.OrderBy(columns) : query.OrderByDesc(columns);

        public static Query OrderBy(this Query query, string column, OrderDirection orderDirection)
            => query.OrderBy(orderDirection, column);
    }
}
