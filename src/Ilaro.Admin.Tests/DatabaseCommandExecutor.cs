using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Tests
{
    public static class DatabaseCommandExecutor
    {
        public static void ExecuteScript(string file, IDbConnection connection)
        {
            var sql = File.ReadAllText(file);
            try
            {
                ExecuteSql(sql, connection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error when executing script {0}".Fill(file), ex);
            }
        }

        public static void ExecuteSql(string sql, IDbConnection connection)
        {
            var commands = Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline);

            foreach (var statement in commands.Where(cmd => String.IsNullOrWhiteSpace(cmd) == false))
            {
                ExecuteCommand(connection, statement);
            }
        }

        public static void ExecuteCommand(IDbConnection connection, string statement)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = statement;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
