using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Extensions2;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsCreator : ICreatingRecords
    {
        private readonly Notificator _notificator;
        private readonly IExecutingDbCommand _executor;
        private DB _db;

        private const string SqlFormat =
@"INSERT INTO {0} ({1}) 
VALUES ({2});
DECLARE @newID int = SCOPE_IDENTITY();
SELECT @newID;";

        /// <summary>
        /// UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
        /// </summary>
        private const string RelatedRecordsUpdateSqlFormat =
            "UPDATE {0} SET {1} = @newID WHERE {2} In ({3});";

        public RecordsCreator(Notificator notificator, IExecutingDbCommand executor)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (executor == null)
                throw new ArgumentNullException("executor");

            _notificator = notificator;
            _executor = executor;
        }

        public object Create(Entity entity)
        {
            //FileHandle(entity);
            _db = new DB();

            var cmd = CreateCommand(entity);

            var item = _executor
                .ExecuteWithChanges(cmd, new ChangeInfo(entity.Name, EntityChangeType.Insert));

            entity.ClearPropertiesValues();

            return item;
        }

        private DbCommand CreateCommand(Entity entity)
        {
            var cmd = CreateBaseCommand(entity);

            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var property in entity.GetForeignsForUpdate())
            {
                var values = string.Join(",", property.Value.Values.Select(x => "@" + paramIndex++));
                sbUpdates.AppendLine();
                sbUpdates.AppendFormat(
                    RelatedRecordsUpdateSqlFormat,
                    property.ForeignEntity.TableName,
                    entity.Key.ColumnName,
                    property.ForeignEntity.Key.ColumnName,
                    values);
                cmd.AddParams(property.Value.Values);
            }

            cmd.CommandText += sbUpdates.ToString();

            return cmd;
        }

        private DbCommand CreateBaseCommand(Entity entity)
        {
            var sbKeys = new StringBuilder();
            var sbVals = new StringBuilder();

            var cmd = _db.CreateCommand();
            var counter = 0;
            foreach (var property in entity.CreateProperties(getForeignCollection: false))
            {
                sbKeys.AppendFormat("{0},", property.ColumnName);
                sbVals.AppendFormat("@{0},", counter);
                AddParam(cmd, property);
                counter++;
            }
            var keys = sbKeys.ToString().Substring(0, sbKeys.Length - 1);
            var vals = sbVals.ToString().Substring(0, sbVals.Length - 1);
            var sql = string.Format(SqlFormat, entity.TableName, keys, vals);
            cmd.CommandText = sql;

            return cmd;
        }

        private static void AddParam(DbCommand cmd, Property property)
        {
            if (property.TypeInfo.IsFileStoredInDb)
                cmd.AddParam(property.Value.Raw, DbType.Binary);
            else
                cmd.AddParam(property.Value.Raw);
        }

        //private void FileHandle(Entity entity)
        //{
        //    foreach (var property in entity
        //        .CreateProperties(getForeignCollection: false)
        //        .Where(x => x.TypeInfo.DataType == DataType.File))
        //    {
        //        if (property.TypeInfo.Type == typeof(string))
        //        {
        //            // we must save file to disk and save file path in db
        //            var file = (HttpPostedFile)property.Value.Raw;
        //            var fileName = String.Empty;
        //            if (property.ImageOptions.NameCreation == NameCreation.UserInput)
        //            {
        //                fileName = "test.jpg";
        //            }
        //            fileName = FileUpload.SaveImage(file, fileName, property.ImageOptions.NameCreation, property.ImageOptions.Settings.ToArray());

        //            property.Value.Raw = fileName;
        //        }
        //        else
        //        {
        //            // we must save file in db as byte array

        //            var file = (HttpPostedFile)property.Value.Raw;
        //            var bytes = FileUpload.GetImageByte(file, property.ImageOptions.Settings.ToArray());
        //            property.Value.Raw = bytes;
        //        }
        //    }
        //}
    }
}