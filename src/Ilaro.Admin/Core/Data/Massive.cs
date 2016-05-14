using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using Ilaro.Admin;

namespace Massive
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Extension method for adding in a bunch of parameters
        /// </summary>
        public static void AddParams(this DbCommand cmd, params object[] args)
        {
            foreach (var item in args)
            {
                AddParam(cmd, item);
            }
        }
        /// <summary>
        /// Extension for adding single parameter
        /// </summary>
        public static void AddParam(this DbCommand cmd, object item, DbType? type = null)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("@{0}", cmd.Parameters.Count);
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                if (item.GetType() == typeof(Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 4000;
                }
                else if (item.GetType() == typeof(ExpandoObject))
                {
                    var d = (IDictionary<string, object>)item;
                    p.Value = d.Values.FirstOrDefault();
                }
                else
                {
                    p.Value = item;
                    if (type.HasValue)
                        p.DbType = type.Value;
                }
                if (item.GetType() == typeof(string))
                    p.Size = ((string)item).Length > 4000 ? -1 : 4000;
            }
            cmd.Parameters.Add(p);
        }
        public static IDictionary<string, object> RecordToDictionary(this IDataReader rdr)
        {
            var d = new Dictionary<string, object>();
            object[] values = new object[rdr.FieldCount];
            rdr.GetValues(values);
            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];
                d.Add(rdr.GetName(i), DBNull.Value.Equals(v) ? null : v);
            }
            return d;
        }
    }

    /// <summary>
    /// A class that wraps your database table in Dynamic Funtime
    /// </summary>
    public class DynamicModel : DynamicObject
    {
        DbProviderFactory _factory;
        string ConnectionString;

        public DynamicModel(string connectionStringName, string tableName = "",
            string primaryKeyField = "", string descriptorField = "")
        {
            TableName = tableName == "" ? this.GetType().Name : tableName;
            PrimaryKeyField = string.IsNullOrEmpty(primaryKeyField) ? "ID" : primaryKeyField;
            DescriptorField = descriptorField;
            var _providerName = "System.Data.SqlClient";

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                _providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;

            _factory = DbProviderFactories.GetFactory(_providerName);
            ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            KeyColSeparator = Const.KeyColSeparator;
        }

        public string DescriptorField { get; protected set; }

        /// <summary>
        /// Enumerates the reader yielding the result - thanks to Jeroen Haegebaert
        /// </summary>
        public virtual IEnumerable<IDictionary<string, object>> Query(string sql, params object[] args)
        {
            using (var conn = OpenConnection())
            {
                var rdr = CreateCommand(sql, conn, args).ExecuteReader();
                while (rdr.Read())
                {
                    yield return rdr.RecordToDictionary();
                }
            }
        }
        /// <summary>
        /// Returns a single result
        /// </summary>
        public virtual object Scalar(string sql, params object[] args)
        {
            object result = null;
            using (var conn = OpenConnection())
            {
                result = CreateCommand(sql, conn, args).ExecuteScalar();
            }
            return result;
        }
        /// <summary>
        /// Creates a DBCommand that you can use for loving your database.
        /// </summary>
        DbCommand CreateCommand(string sql, DbConnection conn, params object[] args)
        {
            var result = _factory.CreateCommand();
            result.Connection = conn;
            result.CommandText = sql;
            if (args.Length > 0)
                result.AddParams(args);
            return result;
        }
        /// <summary>
        /// Returns and OpenConnection
        /// </summary>
        public virtual DbConnection OpenConnection()
        {
            var result = _factory.CreateConnection();
            result.ConnectionString = ConnectionString;
            result.Open();
            return result;
        }

        public virtual string PrimaryKeyField { get; set; }
        public virtual char KeyColSeparator { get; set; }
        public virtual string TableName { get; set; }
        /// <summary>
        /// Returns all records complying with the passed-in WHERE clause and arguments, 
        /// ordered as specified, limited (TOP) by limit.
        /// </summary>
        public virtual IEnumerable<IDictionary<string, object>> All(string where = "", string orderBy = "", int limit = 0, string columns = "*", params object[] args)
        {
            string sql = BuildSelect(where, orderBy, limit);
            return Query(string.Format(sql, columns, TableName), args);
        }
        private static string BuildSelect(string where, string orderBy, int limit)
        {
            string sql = limit > 0 ? "SELECT TOP " + limit + " {0} FROM {1} " : "SELECT {0} FROM {1} ";
            if (!string.IsNullOrEmpty(where))
                sql += where.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase) ? where : " WHERE " + where;
            if (!String.IsNullOrEmpty(orderBy))
                sql += orderBy.Trim().StartsWith("order by", StringComparison.OrdinalIgnoreCase) ? orderBy : " ORDER BY " + orderBy;
            return sql;
        }

        /// <summary>
        /// Returns a dynamic PagedResult. Result properties are Items, TotalPages, and TotalRecords.
        /// </summary>
        public virtual dynamic Paged(string where = "", string orderBy = "", string columns = "*", int pageSize = 20, int currentPage = 1, params object[] args)
        {
            return BuildPagedResult(where: where, orderBy: orderBy, columns: columns, pageSize: pageSize, currentPage: currentPage, args: args);
        }

        private dynamic BuildPagedResult(string primaryKeyField = "", string where = "", string orderBy = "", string columns = "*", int pageSize = 20, int currentPage = 1, params object[] args)
        {
            dynamic result = new ExpandoObject();
            var countSQL = string.Format("SELECT COUNT({0}) FROM {1}", PrimaryKeyField.Split(KeyColSeparator).FirstOrDefault(), TableName);

            if (String.IsNullOrEmpty(orderBy))
            {
                orderBy = string.IsNullOrEmpty(primaryKeyField) ? PrimaryKeyField : primaryKeyField;
            }

            if (!string.IsNullOrEmpty(where))
            {
                if (!where.Trim().StartsWith("where", StringComparison.CurrentCultureIgnoreCase))
                {
                    where = " WHERE " + where;
                }
            }

            var query = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS Row, {0} FROM {2} {3}) AS {2} ", columns, orderBy, TableName, where);

            var pageStart = (currentPage - 1) * pageSize;
            query += string.Format(" WHERE Row > {0} AND Row <={1}", pageStart, (pageStart + pageSize));
            countSQL += where;
            result.TotalRecords = Scalar(countSQL, args);
            result.TotalPages = result.TotalRecords / pageSize;
            if (result.TotalRecords % pageSize > 0)
                result.TotalPages += 1;
            result.Items = Query(string.Format(query, columns, TableName), args);
            return result;
        }

        /// <summary>
        /// Returns a single row from the database
        /// </summary>
        public virtual dynamic Single(string columns, params object[] key)
        {
            var sql = string.Format("SELECT {0} FROM {1}", columns, TableName);
            var primaryKeyElem = PrimaryKeyField.Split(KeyColSeparator).Select(x => x.Trim()).ToArray();
            for (var i = 0; i < primaryKeyElem.Length; i++)
            {
                if (i == 0) sql = sql + " WHERE " + primaryKeyElem[i] + " = @" + i;
                else sql = sql + " AND " + primaryKeyElem[i] + " = @" + i;
            }
            var items = Query(sql, key).ToList();
            return items.FirstOrDefault();
        }

        public virtual dynamic Single(params object[] key)
        {
            return Single("*", key);
        }
    }
}