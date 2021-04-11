﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Ilaro.Admin.Core.Extensions;
using Massive;

namespace Ilaro.Admin.Core.DataAccess
{
    public class RecordCreator : IRecordCreator
    {
        //private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(RecordCreator));
        private readonly IIlaroAdmin _admin;
        private readonly ICommandExecutor _executor;
        private readonly IUser _user;

        public RecordCreator(
            IIlaroAdmin admin,
            ICommandExecutor executor,
            IUser user)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _admin = admin;
            _executor = executor;
            _user = user;
        }

        public string Create(EntityRecord entityRecord, Func<string> changeDescriber = null)
        {
            try
            {
                var cmd = CreateCommand(entityRecord);

                var result = _executor.ExecuteWithChanges(
                    cmd,
                    entityRecord,
                    EntityChangeType.Insert,
                    changeDescriber);

                return result.ToStringSafe();
            }
            catch (Exception ex)
            {
                //_log.Error(ex);
                throw;
            }
        }

        private DbCommand CreateCommand(EntityRecord entityRecord)
        {
            var cmd = CreateBaseCommand(entityRecord);
            AddForeignsUpdate(cmd, entityRecord);
            AddManyToManyForeignsUpdate(cmd, entityRecord);

            return cmd;
        }

        private DbCommand CreateBaseCommand(EntityRecord entityRecord)
        {
            var sbColumns = new StringBuilder();
            var sbValues = new StringBuilder();

            var cmd = DB.CreateCommand(_admin.ConnectionStringName);
            var counter = 0;
            foreach (var propertyValue in entityRecord.Values
                .WhereIsNotSkipped()
                .WhereIsNotOneToMany()
                .DistinctBy(x => x.Property.Column)
                .Where(x => x.Property.IsAutoKey == false))
            {
                sbColumns.AppendFormat("{0},", propertyValue.Property.Column);
                sbValues.AppendFormat("@{0},", counter);
                AddParam(cmd, propertyValue);
                counter++;
            }
            var columns = sbColumns.ToString().Substring(0, sbColumns.Length - 1);
            var values = sbValues.ToString().Substring(0, sbValues.Length - 1);
            var idType = "int";
            var insertedId = "SCOPE_IDENTITY()";
            if (entityRecord.Keys.Count > 1 || entityRecord.Keys.FirstOrDefault().Property.TypeInfo.IsString)
            {
                idType = "nvarchar(max)";
                insertedId = "@" + counter;
                cmd.AddParam(entityRecord.JoinedKeysValues);
            }
            var table = entityRecord.Entity.Table;

            cmd.CommandText =
$@"-- insert record
INSERT INTO {table} ({columns}) 
VALUES ({values});
-- return record id
DECLARE @newID {idType} = {insertedId};
SELECT @newID;
-- update foreign entities records";

            return cmd;
        }

        private void AddForeignsUpdate(DbCommand cmd, EntityRecord entityRecord)
        {
            if (entityRecord.Keys.Count > 1)
                return;
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var propertyValue in entityRecord.Values
                .WhereOneToMany()
                .Where(x => x.Property.IsManyToMany == false)
                .Where(value => value.Values.IsNullOrEmpty() == false))
            {
                var values =
                    propertyValue.Values.Select(
                        x => x.ToStringSafe().Split(Const.KeyColSeparator).Select(y => y.Trim()).ToList()).ToList();
                var whereParts = new List<string>();
                var addSqlPart = true;
                for (int i = 0; i < propertyValue.Property.ForeignEntity.Keys.Count; i++)
                {
                    var key = propertyValue.Property.ForeignEntity.Keys[i];
                    var joinedValues = string.Join(",", values.Select(x => "@" + paramIndex++));
                    whereParts.Add("{0} In ({1})".Fill(key.Column, joinedValues));
                    addSqlPart = joinedValues.HasValue();
                    if (addSqlPart == false)
                        break;
                    cmd.AddParams(values.Select(x => x[i]).OfType<object>().ToArray());
                }
                if (addSqlPart)
                {
                    var constraintSeparator = Environment.NewLine + "   AND ";
                    var constraints = string.Join(constraintSeparator, whereParts);
                    sbUpdates.AppendLine();

                    var table = propertyValue.Property.ForeignEntity.Table;
                    var foreignKey = entityRecord.Entity.Keys.FirstOrDefault().Column;

                    sbUpdates.Append($@"UPDATE {table}
   SET {foreignKey} = @newID 
 WHERE {constraints};");
                }
            }

            cmd.CommandText += sbUpdates.ToString();
        }

        private void AddManyToManyForeignsUpdate(DbCommand cmd, EntityRecord entityRecord)
        {
            if (entityRecord.Keys.Count > 1)
                return;
            var sbUpdates = new StringBuilder();
            var paramIndex = cmd.Parameters.Count;
            foreach (var propertyValue in entityRecord.Values.WhereOneToMany()
                .Where(x => x.Property.IsManyToMany))
            {
                var selectedValues = propertyValue.Values.Select(x => x.ToStringSafe()).ToList();

                var mtmEntity = GetEntityToLoad(propertyValue.Property);

                var idsToAdd = selectedValues
                    .ToList();
                if (idsToAdd.Any())
                {
                    sbUpdates.AppendLine();
                    sbUpdates.AppendLine("-- add many to many records");
                    foreach (var idToAdd in idsToAdd)
                    {
                        var foreignEntity = propertyValue.Property.ForeignEntity;
                        var key1 =
                            foreignEntity.ForeignKeys.FirstOrDefault(
                                x => x.ForeignEntity == propertyValue.Property.Entity);
                        var key2 =
                            foreignEntity.ForeignKeys.FirstOrDefault(
                                x => x.ForeignEntity == mtmEntity);
                        cmd.AddParam(idToAdd);
                        sbUpdates.AppendLine($"INSERT INTO {foreignEntity.Table} ({key1.Column}, {key2.Column}) VALUES(@newID, @{paramIndex++})");
                    }
                }
            }

            cmd.CommandText += Environment.NewLine + sbUpdates;
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

        private void AddParam(DbCommand cmd, PropertyValue propertyValue)
        {
            if (propertyValue.Raw is ValueBehavior)
            {
                switch (propertyValue.Raw as ValueBehavior?)
                {
                    case ValueBehavior.Now:
                        cmd.AddParam(DateTime.Now);
                        break;
                    case ValueBehavior.UtcNow:
                        cmd.AddParam(DateTime.UtcNow);
                        break;
                    case ValueBehavior.Guid:
                        cmd.AddParam(Guid.NewGuid());
                        break;
                    case ValueBehavior.CurrentUserId:
                        cmd.AddParam((int)_user.Id());
                        break;
                    case ValueBehavior.CurrentUserName:
                        cmd.AddParam(_user.UserName());
                        break;
                }
            }
            else
            {
                if (propertyValue.Property.TypeInfo.IsFileStoredInDb)
                    cmd.AddParam(propertyValue.Raw, DbType.Binary);
                else
                    cmd.AddParam(propertyValue.Raw);
            }
        }
    }
}