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
        public static void ClearDatabase(IDbConnection connection)
        {
            ExecuteCommand(connection, @"
EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'            

EXEC sp_MSForEachTable '
 IF OBJECTPROPERTY(object_id(""?""), ""TableHasForeignRef"") = 1  
  DELETE FROM ?  
 else   
  TRUNCATE TABLE ?  
'

EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'

EXEC sp_MSForEachTable '   
-- Use sys.identity_columns to see if there was a last known identity value
-- for the Table. If there was one, the Table is not new and needs a reset
-- We do not reset by default because a different initial seed is created depending
-- on whether table thad rows in it which breaks tests
IF (OBJECTPROPERTY(object_id(""?""), ""TableHasIdentity"") = 1) AND (EXISTS (SELECT 1 FROM sys.identity_columns WHERE OBJECT_ID = object_id(""?"") AND last_value IS NOT NULL) )
begin
	declare @name varchar(1000) = object_name(object_id(""?""))
    DBCC CHECKIDENT (@name, RESEED, 0)
end
'");
        }

        public static void ExecuteScript(string file, IDbConnection connection)
        {
            var sql = File.ReadAllText(file);
            try
            {
                ExecuteSql(sql, connection);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException("Error when executing script {0}".Fill(file), exc);
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
