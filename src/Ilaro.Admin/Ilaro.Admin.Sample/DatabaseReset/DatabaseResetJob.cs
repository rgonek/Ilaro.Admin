using NLog;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.Hosting;
using System.Web.Configuration;

namespace Ilaro.Admin.Sample.DatabaseReset
{
    public class DatabaseResetJob
    {
        static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void Execute()
        {
            _log.Info("Start resetting database");

            ResetDatabase();
            SetLastDatabaseResetDate();

            _log.Info("End resetting database");
        }

        private static void ResetDatabase()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NorthwindEntities"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var commands = GetCommandsFromFile("ClearDb.sql");
                        foreach (var commandText in commands)
                        {
                            using (var command = new SqlCommand(commandText, connection, tx))
                                command.ExecuteNonQuery();
                        }

                        _log.Info("Database cleared");

                        commands = GetCommandsFromFile("InsertData.sql");
                        foreach (var commandText in commands)
                        {
                            using (var command = new SqlCommand(commandText, connection, tx))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        _log.Info("Records inserted");

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, "Exception occured during resetting database.");
                        tx.Rollback();
                    }
                }
            }
        }

        private static void SetLastDatabaseResetDate()
        {
            _log.Info("Saving last reset date to app settings");
            var webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");
            webConfigApp.AppSettings.Settings["LastDatabaseReset"].Value = DateTime.UtcNow.ToString();
            webConfigApp.Save();
        }

        private static string[] GetCommandsFromFile(string fileName)
        {
            var text = File.ReadAllText(HostingEnvironment.MapPath(@"~\DatabaseReset\scripts\" + fileName));
            var commands = text.Split(new[] { "\rGO\r", "\r\nGO\r" }, StringSplitOptions.RemoveEmptyEntries);

            return commands;
        }
    }
}