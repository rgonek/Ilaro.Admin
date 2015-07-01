using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Web;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Data;
using Ilaro.Admin.Core.FileUpload;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Filters;
using Ilaro.Admin.Models;
using Massive;
using Resources;
using DataType = Ilaro.Admin.Core.DataType;

namespace Ilaro.Admin.Services
{
    public class EntityService : IEntityService
    {
        private readonly Notificator _notificator;
        private readonly IFetchingEntitiesRecords _entitiesSource;

        public EntityService(
            Notificator notificator,
            IFetchingEntitiesRecords entitiesSource)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");
            if (entitiesSource == null)
                throw new ArgumentNullException("entitiesSource");

            _notificator = notificator;
            _entitiesSource = entitiesSource;
        }

        public object Create(Entity entity)
        {
            var existingItem = _entitiesSource
                .GetRecord(entity, entity.Key.Value.AsObject);
            if (existingItem != null)
            {
                _notificator.Error(IlaroAdminResources.EntityAlreadyExist);
                return null;
            }

            FileHandle(entity);

            var table = new DynamicModel(
                AdminInitialise.ConnectionString,
                entity.TableName,
                entity.Key.ColumnName);

            var expando = new ExpandoObject();
            var filler = (IDictionary<String, object>)expando;
            foreach (var property in entity.CreateProperties(getForeignCollection: false))
            {
                filler[property.ColumnName] = property.Value.Raw;
            }
            var cmd = table.CreateInsertCommand(expando);
            cmd.CommandText +=
                Environment.NewLine +
                ";DECLARE @newID int; SELECT @newID = SCOPE_IDENTITY()";
            foreach (var property in entity.Properties
                .Where(x =>
                    x.IsForeignKey &&
                    x.TypeInfo.IsCollection &&
                    !x.Value.Values.IsNullOrEmpty()))
            {
                // UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
                var updateFormat =
                    Environment.NewLine +
                    ";UPDATE {0} SET {1} = @newID WHERE {2} In ({3})";
                cmd.CommandText += updateFormat.Fill(
                    property.ForeignEntity.TableName,
                    entity.Key.ColumnName,
                    property.ForeignEntity.Key.ColumnName,
                    DecorateSqlValue(property.Value.Values,
                    property.ForeignEntity.Key));
            }
            cmd.CommandText += Environment.NewLine + ";SELECT @newID";

            FixParamsSqlType(entity, cmd, filler);

            var item = table.Execute(cmd);

            AddEntityChange(entity.Name, item.ToString(), EntityChangeType.Insert);

            entity.ClearProperties();

            return item;
        }

        private void FixParamsSqlType(
            Entity entity,
            DbCommand cmd,
            IDictionary<string, object> values)
        {
            foreach (var property in entity.CreateProperties(getForeignCollection: false)
                .Where(x => x.TypeInfo.DataType == DataType.File))
            {
                if (!property.TypeInfo.IsString)
                {
                    var index = GetIndex(values, property.ColumnName);

                    var parameter = cmd.Parameters
                        .OfType<SqlParameter>()
                        .FirstOrDefault(x => x.ParameterName == "@" + index);
                    parameter.SqlDbType = System.Data.SqlDbType.Image;
                }
            }
        }

        private int GetIndex(IDictionary<string, object> values, string key)
        {
            var index = 0;
            foreach (var item in values)
            {
                if (item.Key == key)
                {
                    return index;
                }

                index++;
            }
            return -1;
        }

        private void FileHandle(Entity entity)
        {
            foreach (var property in entity
                .CreateProperties(getForeignCollection: false)
                .Where(x => x.TypeInfo.DataType == DataType.File))
            {
                if (property.TypeInfo.Type == typeof(string))
                {
                    // we must save file to disk and save file path in db
                    var file = (HttpPostedFile)property.Value.Raw;
                    var fileName = String.Empty;
                    if (property.ImageOptions.NameCreation == NameCreation.UserInput)
                    {
                        fileName = "test.jpg";
                    }
                    fileName = FileUpload.SaveImage(file, fileName, property.ImageOptions.NameCreation, property.ImageOptions.Settings.ToArray());

                    property.Value.Raw = fileName;
                }
                else
                {
                    // we must save file in db as byte array

                    var file = (HttpPostedFile)property.Value.Raw;
                    var bytes = FileUpload.GetImageByte(file, property.ImageOptions.Settings.ToArray());
                    property.Value.Raw = bytes;
                }
            }
        }

        public int Edit(Entity entity)
        {
            if (entity.Key.Value.Raw == null)
            {
                _notificator.Error(IlaroAdminResources.EntityKeyIsNull);
                return 0;
            }

            var existingItem = _entitiesSource.GetRecord(entity, entity.Key.Value.AsObject);
            if (existingItem == null)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return 0;
            }

            var table = new DynamicModel(
                AdminInitialise.ConnectionString,
                entity.TableName,
                entity.Key.ColumnName);

            var expando = new ExpandoObject();
            var filler = expando as IDictionary<String, object>;
            foreach (var property in entity.CreateProperties(getForeignCollection: false))
            {
                filler[property.ColumnName] = property.Value.Raw;
            }
            var cmd = table.CreateUpdateCommand(expando, entity.Key.Value.Raw);
            foreach (var property in entity.Properties.Where(x => x.IsForeignKey && x.TypeInfo.IsCollection))
            {
                var actualRecords = _entitiesSource.GetRecords(
                    property.ForeignEntity,
                    new List<IEntityFilter>
                    {
                        new ForeignEntityFilter(
                            entity.Key, 
                            entity.Key.Value.Raw.ToStringSafe())
                    });
                var idsToRemoveRelation = actualRecords
                    .Select(x => x.KeyValue)
                    .Except(property.Value.Values.OfType<string>())
                    .ToList();
                // UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
                var updateFormat =
                    Environment.NewLine +
                    ";UPDATE {0} SET {1} = {2} WHERE {3} In ({4})";
                if (idsToRemoveRelation.Any())
                {
                    cmd.CommandText +=
                        Environment.NewLine +
                        updateFormat.Fill(
                            property.ForeignEntity.TableName,
                            entity.Key.ColumnName,
                            "NULL",
                            property.ForeignEntity.Key.ColumnName,
                            DecorateSqlValue(
                                idsToRemoveRelation.OfType<object>().ToList(),
                                property.ForeignEntity.Key));
                }
                cmd.CommandText +=
                    Environment.NewLine +
                    updateFormat.Fill(
                        property.ForeignEntity.TableName,
                        entity.Key.ColumnName,
                        DecorateSqlValue(entity.Key.Value.Raw, entity.Key),
                        property.ForeignEntity.Key.ColumnName,
                        DecorateSqlValue(property.Value.Values, property.ForeignEntity.Key));
            }
            var savedItems = table.Execute(cmd);

            // TODO: get info about changed properties
            AddEntityChange(entity.Name, entity.Key.Value.AsString, EntityChangeType.Update);

            entity.ClearProperties();

            return savedItems;
        }

        private string DecorateSqlValue(IList<object> values, Property property)
        {
            if (property.TypeInfo.DataType == DataType.Numeric)
            {
                return string.Join(",", values);
            }

            return "'" + string.Join("','", values) + "'";
        }

        private string DecorateSqlValue(object value, Property property)
        {
            if (property.TypeInfo.DataType == DataType.Numeric)
            {
                return value.ToStringSafe();
            }

            return "'" + value.ToStringSafe() + "'";
        }

        private void FillEntity(object item, Entity entity)
        {
            var request = HttpContext.Current.Request;
            foreach (var property in entity.CreateProperties(false))
            {
                if (property.TypeInfo.DataType == DataType.File)
                {
                    var file = request.Files[property.Name];
                    var fileName = String.Empty;
                    if (property.ImageOptions.NameCreation == NameCreation.UserInput)
                    {
                        fileName = "test.jpg";
                    }
                    fileName = FileUpload.SaveImage(file,
                        fileName,
                        property.ImageOptions.NameCreation,
                        property.ImageOptions.Settings.ToArray());

                    property.Value.Raw = fileName;
                }

                var propertyInfo = entity.Type.GetProperty(property.Name);
                propertyInfo.SetValue(item, property.Value.Raw, null);
            }
        }

        public bool Delete(Entity entity, string key, IEnumerable<PropertyDeleteOption> propertiesDeleteOptions)
        {
            var deleteOptions = propertiesDeleteOptions.ToDictionary(x => x.PropertyName, x => x.DeleteOption);
            foreach (var property in entity.Properties.Where(x => x.IsForeignKey && x.TypeInfo.IsCollection))
            {
                if (!deleteOptions.ContainsKey(property.ForeignEntity.Name))
                {
                    deleteOptions[property.ForeignEntity.Name] = property.DeleteOption;
                }
            }

            var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.ColumnName);

            var keyObject = entity.Key.Value.ToObject(key);
            var result = 0;

            if (deleteOptions.All(x => x.Value == DeleteOption.Nothing))
            {
                result = table.Delete(keyObject);
            }
            else
            {
                var sql = String.Empty;
                var recordHierarchy = GetRecordHierarchy(entity);
                foreach (var subRecord in recordHierarchy.SubRecordsHierarchies)
                {
                    var deleteOption = DeleteOption.Nothing;
                    if (deleteOptions.ContainsKey(subRecord.Entity.Name))
                    {
                        deleteOption = deleteOptions[subRecord.Entity.Name];
                    }
                    if (deleteOption == DeleteOption.SetNull)
                    {
                        sql += GetSetToNullUpdateSql(entity, subRecord) + Environment.NewLine;
                    }
                    else if (deleteOption == DeleteOption.CascadeDelete)
                    {
                        sql += string.Join(Environment.NewLine, GetDeleteRelatedEntityDeleteSql(subRecord).Reverse()) + Environment.NewLine;
                    }
                }
                var cmd = table.CreateDeleteCommand(key: keyObject);
                cmd.CommandText = sql + cmd.CommandText;

                result = table.Execute(cmd);
            }

            if (result < 1)
            {
                _notificator.Error(IlaroAdminResources.EntityNotExist);
                return false;
            }

            AddEntityChange(entity.Name, key, EntityChangeType.Delete);

            return true;
        }

        private IList<string> GetDeleteRelatedEntityDeleteSql(RecordHierarchy record)
        {
            var sqls = new List<string>();

            // {0} - Foreign table
            // {1} - Primary key
            // {2} - Key value
            var deleteFormat = "DELETE FROM {0} WHERE {1} = {2};";

            sqls.Add(string.Format(deleteFormat, record.Entity.TableName, record.Entity.Key.ColumnName, DecorateSqlValue(record.KeyValue, record.Entity.Key)));

            foreach (var subRecord in record.SubRecordsHierarchies)
            {
                sqls.AddRange(GetDeleteRelatedEntityDeleteSql(subRecord));
            }

            return sqls;
        }

        private string GetSetToNullUpdateSql(Entity entity, RecordHierarchy subRecord)
        {
            // {0} - Foreign table
            // {1} - Foreign key
            // {2} - Primary key
            // {3} - Key value
            var updateFormat = "UPDATE {0} SET {1} = NULL WHERE {2} = {3};";
            //UPDATE Products SET CategoryID = null WHERE ProductID = 7

            var foreignTable = subRecord.Entity.TableName;
            var foreignKey = subRecord.Entity.Properties.FirstOrDefault(x => x.ForeignEntity == entity).ColumnName;
            var primaryKey = subRecord.Entity.Key.ColumnName;
            var keyValue = DecorateSqlValue(subRecord.KeyValue, subRecord.Entity.Key);

            var updateSql = string.Format(updateFormat, foreignTable, foreignKey, primaryKey, keyValue);

            return updateSql;
        }

        public object GetKeyValue(Entity entity, object savedItem)
        {
            return ((dynamic)savedItem).ID;
        }

        public IList<GroupProperties> PrepareGroups(Entity entity, bool getKey = true, string key = null)
        {
            var properties = entity.CreateProperties(getKey);
            foreach (var foreign in properties.Where(x => x.IsForeignKey))
            {
                var records = _entitiesSource.GetRecords(foreign.ForeignEntity, determineDisplayValue: true);
                foreign.Value.PossibleValues = records.ToDictionary(x => x.KeyValue, x => x.DisplayName);
                if (foreign.TypeInfo.IsCollection)
                {
                    foreign.Value.Values = records.Where(x => x.Values.Any(y => y.Property.ForeignEntity == entity && y.AsString == key)).Select(x => x.KeyValue).OfType<object>().ToList();
                }
            }

            return entity.Groups;
        }

        /// <summary>
        /// Save record with information about entity change
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <param name="entityKey">Entity key of changed record</param>
        /// <param name="changeType">Change type</param>
        /// <param name="description">Description for update change type</param>
        private void AddEntityChange(string entityName, string entityKey, EntityChangeType changeType, string description = null)
        {
            if (AdminInitialise.ChangeEntity == null)
            {
                return;
            }

            var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: AdminInitialise.ChangeEntity.TableName, primaryKeyField: "EntityChangeId");

            dynamic changeRecord = new ExpandoObject();
            changeRecord.EntityName = entityName;
            changeRecord.EntityKey = entityKey;
            changeRecord.ChangeType = changeType;
            changeRecord.Description = description;
            changeRecord.ChangedOn = DateTime.UtcNow;
            changeRecord.ChangedBy = HttpContext.Current.User.Identity.Name;

            table.Insert(changeRecord);
        }

        public RecordHierarchy GetRecordHierarchy(Entity entity)
        {
            var index = 0;
            var hierarchy = GetEntityHierarchy(null, entity, ref index);
            var sql = GenerateHierarchySQL(hierarchy);
            var model = new DynamicModel(AdminInitialise.ConnectionString);
            var records = model.Query(sql, entity.Key.Value.Raw);

            var recordHierarchy = GetHierarchyRecords(records, hierarchy);

            return recordHierarchy;

            // TODO: test it
            //			SELECT [t0].*, [t1].*, [t2].*
            //FROM [Categories] AS [t0]
            //LEFT OUTER JOIN [Products] AS [t1] ON [t1].[CategoryID] = [t0].[CategoryID]
            //LEFT OUTER JOIN [Suppliers] AS [t2] ON [t2].[SupplierID] = [t1].[SupplierID]
            //WHERE [t0].CategoryID = 9
            //ORDER BY [t0].[CategoryID], [t1].[ProductID], [t2].[SupplierID]
        }

        private RecordHierarchy GetHierarchyRecords(IEnumerable<dynamic> records, EntityHierarchy hierarchy)
        {
            var baseRecord = records.FirstOrDefault();
            var prefix = hierarchy.Alias.Trim("[]".ToCharArray()) + "_";
            var rowData = new DataRow(baseRecord, hierarchy.Entity, prefix);

            var recordHierarchy = new RecordHierarchy
            {
                Entity = hierarchy.Entity,
                KeyValue = rowData.KeyValue,
                DisplayName = hierarchy.Entity.ToString(rowData),
                SubRecordsHierarchies = new List<RecordHierarchy>()
            };

            GetHierarchyRecords(recordHierarchy, records, hierarchy.SubHierarchies);

            return recordHierarchy;
        }

        private void GetHierarchyRecords(RecordHierarchy parentHierarchy, IEnumerable<dynamic> records, IList<EntityHierarchy> subHierarchies, string foreignKey = null, string foreignKeyValue = null)
        {
            foreach (var hierarchy in subHierarchies)
            {
                var prefix = hierarchy.Alias.Trim("[]".ToCharArray()) + "_";

                foreach (var record in records)
                {
                    var recordDict = (IDictionary<string, object>)record;
                    var rowData = new DataRow(recordDict, hierarchy.Entity, prefix);

                    if (!rowData.KeyValue.IsNullOrEmpty())
                    {
                        var subRecord = new RecordHierarchy
                        {
                            Entity = hierarchy.Entity,
                            KeyValue = rowData.KeyValue,
                            DisplayName = hierarchy.Entity.ToString(rowData),
                            SubRecordsHierarchies = new List<RecordHierarchy>()
                        };

                        if (parentHierarchy.SubRecordsHierarchies.FirstOrDefault(x => x.KeyValue == subRecord.KeyValue) == null &&
                            (foreignKey.IsNullOrEmpty() || recordDict[foreignKey].ToStringSafe() == foreignKeyValue))
                        {
                            parentHierarchy.SubRecordsHierarchies.Add(subRecord);

                            GetHierarchyRecords(subRecord, records, hierarchy.SubHierarchies, prefix + hierarchy.Entity.Key.ColumnName, rowData.KeyValue);
                        }
                    }
                }
            }
        }

        private string GenerateHierarchySQL(EntityHierarchy hierarchy)
        {
            var flatHierarchy = FlatHierarchy(hierarchy);

            // {0} - Columns
            // {1} - Base table
            // {2} - Base table alias
            // {3} - Joins
            // {4} - Where
            // {5} - Order by
            var selectFormat = @"SELECT {0} 
FROM {1} AS {2}
{3}
WHERE {4}
ORDER BY {5}";
            // {0} - Foreign table
            // {1} - Foreign alias
            // {2} - Foreign key
            // {3} - Base table alias
            // {4} - Base table primary key
            var joinFormat = @"LEFT OUTER JOIN {0} AS {1} ON {1}.{2} = {3}.{4}";

            var columns = flatHierarchy.SelectMany(x => x.Entity.GetColumns()
                .Select(y => x.Alias + "." + y + " AS " + x.Alias.Trim("[]".ToCharArray()) + "_" + y)).ToList();
            var joins = new List<string>();
            foreach (var item in flatHierarchy.Where(x => x.ParentHierarchy != null))
            {
                var foreignTable = item.Entity.TableName;
                var foreignAlias = item.Alias;
                var foreignKey = String.Empty;
                var baseTableAlias = item.ParentHierarchy.Alias;
                var baseTablePrimaryKey = String.Empty;
                var foreignProperty = item.Entity.Properties.FirstOrDefault(x => x.ForeignEntity == item.ParentHierarchy.Entity);
                if (foreignProperty == null || foreignProperty.TypeInfo.IsCollection)
                {
                    foreignKey = item.Entity.Key.ColumnName;
                    baseTablePrimaryKey = item.ParentHierarchy.Entity.Properties.FirstOrDefault(x => x.ForeignEntity == item.Entity).ColumnName;
                }
                else
                {
                    foreignKey = foreignProperty.ColumnName;
                    baseTablePrimaryKey = item.ParentHierarchy.Entity.Key.ColumnName;
                }
                joins.Add(string.Format(joinFormat, foreignTable, foreignAlias, foreignKey, baseTableAlias, baseTablePrimaryKey));
            }
            var orders = flatHierarchy.Select(x => x.Alias + "." + x.Entity.Key.ColumnName).ToList();

            var where = hierarchy.Alias + "." + hierarchy.Entity.Key.ColumnName + " = @0";

            var sql = string.Format(selectFormat, string.Join(", ", columns), hierarchy.Entity.TableName, hierarchy.Alias, string.Join(Environment.NewLine, joins), where, string.Join(", ", orders));

            return sql;
        }

        private IList<EntityHierarchy> FlatHierarchy(EntityHierarchy hierarchy)
        {
            var flatHierarchy = new List<EntityHierarchy> { hierarchy };

            foreach (var subHierarchy in hierarchy.SubHierarchies)
            {
                flatHierarchy.AddRange(FlatHierarchy(subHierarchy));
            }

            return flatHierarchy;
        }

        private EntityHierarchy GetEntityHierarchy(EntityHierarchy parent, Entity entity, ref int index)
        {
            var hierarchy = new EntityHierarchy
            {
                Entity = entity,
                Alias = "[t" + index + "]",
                SubHierarchies = new List<EntityHierarchy>(),
                ParentHierarchy = parent
            };

            foreach (var property in entity.CreateProperties()
                .Where(x => x.IsForeignKey && x.TypeInfo.IsCollection)
                .Where(property =>
                    parent == null ||
                    parent.Entity != property.ForeignEntity))
            {
                index++;
                hierarchy.SubHierarchies.Add(GetEntityHierarchy(hierarchy, property.ForeignEntity, ref index));
            }

            return hierarchy;
        }
    }
}