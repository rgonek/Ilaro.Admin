using Ilaro.Admin.Services.Interfaces;
using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.EntitiesFilters;
using System.Data.SqlClient;
using System.Data.Entity;
using Ilaro.Admin.Commons;
using System.Data;
using System.Web.Mvc;
using Ilaro.Admin.Commons.Notificator;
using System.Data.Common;
using Massive;
using System.Dynamic;
using System.Collections.Specialized;
using Resources;
using Ilaro.Admin.Model;
using System.Globalization;
using System.Diagnostics;
using Ilaro.Admin.FileUpload;

namespace Ilaro.Admin.Services
{
	public class EntityService : BaseService, IEntityService
	{
		public EntityService(Notificator notificator)
			: base(notificator)
		{
		}

		public PagedRecordsViewModel GetRecords(Entity entity, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
		{
			var skip = (page - 1) * take;

			var search = new EntitySearch { Query = searchQuery, Properties = entity.SearchProperties };
			order = order.IsNullOrEmpty() ? entity.Key.ColumnName : order;
			orderDirection = orderDirection.IsNullOrEmpty() ? "ASC" : orderDirection.ToUpper();
			var orderBy = order + " " + orderDirection;
			var columns = GetColumns(entity);
			var where = ConvertFiltersToSQL(filters, search);

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.ColumnName);

			var result = table.Paged(columns: columns, where: where, orderBy: orderBy, currentPage: page, pageSize: take);

			var data = new List<DataRowViewModel>();
			foreach (var item in result.Items)
			{
				data.Add(ExpandoToDataRow(item, entity));
			}

			return new PagedRecordsViewModel
			{
				TotalItems = result.TotalRecords,
				TotalPages = result.TotalPages,
				Records = data
			};
		}

		private string GetColumns(Entity entity)
		{
			var columns = entity.DisplayProperties.Select(x => x.ColumnName).ToList();
			columns.Insert(0, entity.LinkKey.Name);
			columns.Insert(0, entity.Key.Name);

			return string.Join(",", columns.Distinct());
		}

		public IList<DataRowViewModel> GetRecords(Entity entity, IList<IEntityFilter> filters = null, string searchQuery = null, string order = null, string orderDirection = null, bool determineDisplayValue = false)
		{
			var search = new EntitySearch { Query = searchQuery, Properties = entity.SearchProperties };
			order = order.IsNullOrEmpty() ? entity.Key.ColumnName : order;
			orderDirection = orderDirection.IsNullOrEmpty() ? "ASC" : orderDirection.ToUpper();
			var orderBy = order + " " + orderDirection;
			var columns = string.Join(",", entity.Properties.Where(x => !x.IsForeignKey || (!x.IsCollection && x.IsForeignKey)).Select(x => x.ColumnName).Distinct());
			var where = ConvertFiltersToSQL(filters, search);

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.ColumnName);

			var result = table.All(columns: columns, where: where, orderBy: orderBy);

			var data = new List<DataRowViewModel>();
			foreach (var item in result)
			{
				data.Add(ExpandoToDataRow(item, entity));
			}

			if (determineDisplayValue)
			{
				foreach (var row in data)
				{
					row.DisplayName = GetDisplayName(entity, row);
				}
			}

			return data;
		}

		private DataRowViewModel ExpandoToDataRow(dynamic record, Entity entity, string prefix = null)
		{
			var recordDict = (IDictionary<String, Object>)record;

			return ExpandoToDataRow(recordDict, entity, prefix);
		}

		private DataRowViewModel ExpandoToDataRow(IDictionary<String, Object> recordDict, Entity entity, string prefix = null)
		{
			var row = new DataRowViewModel();
			row.KeyValue = recordDict[prefix + entity.Key.ColumnName].ToStringSafe();
			row.LinkKeyValue = recordDict[prefix + entity.LinkKey.ColumnName].ToStringSafe();
			foreach (var property in entity.DisplayProperties)
			{
				row.Values.Add(new CellValueViewModel
				{
					RawValue = recordDict[prefix + property.ColumnName],
					Value = recordDict[prefix + property.ColumnName].ToStringSafe(property),
					Property = property
				});
			}

			return row;
		}

		/// <summary>
		/// Get display name for entity
		/// </summary>
		/// <param name="entity">Entity</param>
		/// <param name="row">Instance value</param>
		/// <returns>Display name</returns>
		private string GetDisplayName(Entity entity, DataRowViewModel row)
		{
			// check if has to string attribute
			if (!entity.RecordDisplayFormat.IsNullOrEmpty())
			{
				var result = entity.RecordDisplayFormat;
				foreach (var cell in row.Values)
				{
					result = result.Replace("{" + cell.Property.Name + "}", cell.Value);
				}

				return result;
			}
			// if not check if has ToString() method
			else if (entity.HasToStringMethod)
			{
				var methodInfo = entity.Type.GetMethod("ToString");
				var instance = Activator.CreateInstance(entity.Type, null);

				foreach (var cell in row.Values.Where(x => !x.Property.IsForeignKey || (x.Property.IsForeignKey && x.Property.IsSystemType)))
				{
					var propertyInfo = entity.Type.GetProperty(cell.Property.Name);
					propertyInfo.SetValue(instance, cell.RawValue);
				}

				var result = methodInfo.Invoke(instance, null);

				return result.ToStringSafe();
			}
			// if not get first matching property
			// %Name%, %Title%, %Description%, %Value%
			// if not found any property use KeyValue
			var possibleNames = new List<string> { "name", "title", "description", "value" };
			var value = String.Empty;
			foreach (var possibleName in possibleNames)
			{
				var cell = row.Values.FirstOrDefault(x => x.Property.Name.ToLower().Contains("name"));
				if (cell != null)
				{
					value = cell.Value;
					break;
				}
			}

			if (value.IsNullOrEmpty())
			{
				return "#" + row.KeyValue;
			}

			return value;
		}

		public PagedRecordsViewModel GetChangesRecords(Entity entityChangesFor, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
		{
			var skip = (page - 1) * take;
			var changeEntity = AdminInitialise.ChangeEntity;

			var search = new EntitySearch { Query = searchQuery, Properties = changeEntity.SearchProperties };
			order = order.IsNullOrEmpty() ? changeEntity.Key.ColumnName : order;
			orderDirection = orderDirection.IsNullOrEmpty() ? "ASC" : orderDirection.ToUpper();
			var orderBy = order + " " + orderDirection;
			var columns = GetColumns(changeEntity);
			var where = ConvertFiltersToSQL(filters, search);
			if (where.IsNullOrEmpty())
			{
				where += " WHERE EntityName = '" + entityChangesFor.Name + "'";
			}
			else
			{
				where += " AND EntityName = '" + entityChangesFor.Name + "'";
			}
			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: changeEntity.TableName, primaryKeyField: changeEntity.Key.Name);

			var result = table.Paged(columns: columns, where: where, orderBy: orderBy, currentPage: page, pageSize: take);

			var data = new List<DataRowViewModel>();
			foreach (var item in result.Items)
			{
				data.Add(ExpandoToDataRow(item, changeEntity));
			}

			return new PagedRecordsViewModel
			{
				TotalItems = result.TotalRecords,
				TotalPages = result.TotalPages,
				Records = data
			};
		}

		private string ConvertFiltersToSQL(IList<IEntityFilter> filters, EntitySearch search, string alias = "")
		{
			if (filters == null)
			{
				filters = new List<IEntityFilter>();
			}

			var activeFilters = filters.Where(x => !x.Value.IsNullOrEmpty());
			if (!activeFilters.Any() && !search.IsActive)
			{
				return null;
			}

			if (!alias.IsNullOrEmpty())
			{
				alias += ".";
			}

			var conditions = String.Empty;
			foreach (var filter in activeFilters)
			{
				var condition = filter.GetSQLCondition(alias);

				if (!condition.IsNullOrEmpty())
				{
					conditions += string.Format(" {0} AND", filter.GetSQLCondition(alias));
				}
			}

			if (search.IsActive)
			{
				var searchCondition = String.Empty;
				foreach (var property in search.Properties)
				{
					var query = search.Query.TrimStart('>', '<');
					var temp = 0.0m;
					if (property.PropertyType.In(typeof(string)))
					{
						searchCondition += string.Format(" {0}[{1}] LIKE '%{2}%' OR", alias, property.Name, search.Query);
					}
					else if (decimal.TryParse(query, out temp))
					{
						var sign = "=";
						if (search.Query.StartsWith(">"))
						{
							sign = ">=";
						}
						else if (search.Query.StartsWith("<"))
						{
							sign = "<=";
						}

						searchCondition += string.Format(" {0}[{1}] {3} {2} OR", alias, property.Name, query.Replace(",", "."), sign);
					}
				}

				if (!searchCondition.IsNullOrEmpty())
				{
					conditions += " (" + searchCondition.TrimStart(' ').TrimEnd("OR".ToCharArray()) + ")";
				}
			}

			if (conditions.IsNullOrEmpty())
			{
				return null;
			}

			if (conditions.IsNullOrEmpty())
			{
				return String.Empty;
			}
			return " WHERE" + conditions.TrimEnd("AND".ToCharArray());
		}

		public IList<ColumnViewModel> PrepareColumns(Entity entity, string order, string orderDirection)
		{
			if (orderDirection == "asc")
			{
				orderDirection = "up";
			}
			else
			{
				orderDirection = "down";
			}

			order = order.ToLower();

			return entity.DisplayProperties.Select(x => new ColumnViewModel
			{
				Name = x.Name,
				DisplayName = x.DisplayName,
				Description = x.Description,
				SortDirection = x.Name.ToLower() == order ? orderDirection : String.Empty
			}).ToList();
		}

		public object Create(Entity entity)
		{
			var existingItem = GetEntity(entity, entity.Key.Value);
			if (existingItem != null)
			{
				Error(IlaroAdminResources.EntityAlreadyExist);
				return null;
			}

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.ColumnName);

			var expando = new ExpandoObject();
			var filler = expando as IDictionary<String, object>;
			foreach (var property in entity.CreateProperties(getForeignCollection: false))
			{
				filler[property.ColumnName] = property.Value;
			}
			var cmd = table.CreateInsertCommand(expando);
			cmd.CommandText += Environment.NewLine + ";DECLARE @newID int; SELECT @newID = SCOPE_IDENTITY()";
			foreach (var property in entity.Properties.Where(x => x.IsForeignKey && x.IsCollection))
			{
				// UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
				var updateFormat = Environment.NewLine + ";UPDATE {0} SET {1} = @newID WHERE {2} In ({3})";
				cmd.CommandText += string.Format(updateFormat, property.ForeignEntity.TableName, entity.Key.ColumnName, property.ForeignEntity.Key.ColumnName, DecorateSqlValue(property.Values, property.ForeignEntity.Key));
			}
			cmd.CommandText += Environment.NewLine + ";SELECT @newID";
			var item = table.Execute(cmd);

			AddEntityChange(entity.Name, item.ToString(), EntityChangeType.Insert);

			ClearProperties(entity);

			return item;
		}

		public int Edit(Entity entity)
		{
			if (entity.Key.Value == null)
			{
				Error(IlaroAdminResources.EntityKeyIsNull);
				return 0;
			}

			var existingItem = GetEntity(entity, entity.Key.Value);
			if (existingItem == null)
			{
				Error(IlaroAdminResources.EntityNotExist);
				return 0;
			}

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.ColumnName);

			var expando = new ExpandoObject();
			var filler = expando as IDictionary<String, object>;
			foreach (var property in entity.CreateProperties(getForeignCollection: false))
			{
				filler[property.ColumnName] = property.Value;
			}
			var cmd = table.CreateUpdateCommand(expando, entity.Key.Value);
			foreach (var property in entity.Properties.Where(x => x.IsForeignKey && x.IsCollection))
			{
				var actualRecords = GetRecords(property.ForeignEntity, new List<IEntityFilter> { new ForeignEntityFilter(entity.Key, entity.Key.Value.ToStringSafe()) });
				var idsToRemoveRelation = actualRecords.Select(x => x.KeyValue);
				idsToRemoveRelation = idsToRemoveRelation.Except(property.Values.OfType<string>());
				// UPDATE {TableName} SET {ForeignKey} = {FKValue} WHERE {PrimaryKey} In ({PKValues});
				var updateFormat = Environment.NewLine + ";UPDATE {0} SET {1} = {2} WHERE {3} In ({4})";
				if (idsToRemoveRelation.Any())
				{
					cmd.CommandText += Environment.NewLine + string.Format(updateFormat, property.ForeignEntity.TableName, entity.Key.ColumnName, "NULL", property.ForeignEntity.Key.ColumnName, DecorateSqlValue(idsToRemoveRelation.OfType<object>().ToList(), property.ForeignEntity.Key));
				}
				cmd.CommandText += Environment.NewLine + string.Format(updateFormat, property.ForeignEntity.TableName, entity.Key.ColumnName, DecorateSqlValue(entity.Key.Value, entity.Key), property.ForeignEntity.Key.ColumnName, DecorateSqlValue(property.Values, property.ForeignEntity.Key));
			}
			var savedItems = table.Execute(cmd);

			// TODO: get info about changed properties
			AddEntityChange(entity.Name, entity.Key.StringValue, EntityChangeType.Update);

			ClearProperties(entity);

			return savedItems;
		}

		private string DecorateSqlValue(IList<object> values, Property property)
		{
			if (property.DataType == DataType.Numeric)
			{
				return string.Join(",", values);
			}

			return "'" + string.Join("','", values) + "'";
		}

		private string DecorateSqlValue(object value, Property property)
		{
			if (property.DataType == DataType.Numeric)
			{
				return value.ToStringSafe();
			}

			return "'" + value.ToStringSafe() + "'";
		}

		/// <summary>
		/// Clear properties values
		/// </summary>
		public void ClearProperties(Entity entity)
		{
			foreach (var property in entity.Properties)
			{
				property.Value = null;
			}
		}

		public bool ValidateEntity(Entity entity, ModelStateDictionary ModelState)
		{
			bool isValid = true;
			var request = HttpContext.Current.Request;
			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.File))
			{
				var file = request.Files[property.Name];
				var result = FileUpload.FileUpload.Validate(file, property.ImageOptions.MaxFileSize, property.ImageOptions.AllowedFileExtensions, !property.IsRequired);

				if (result != FileUploadValidationResult.Valid)
				{
					isValid = false;
					// TODO: more complex validation message
					ModelState.AddModelError(property.Name, IlaroAdminResources.UnvalidFile);
				}
			}

			foreach (var property in entity.Properties.Where(x => x.DataType != DataType.File))
			{
				foreach (var validator in property.ValidationAttributes)
				{
					try
					{
						validator.Validate(property.Value, property.Name);
					}
					catch (System.ComponentModel.DataAnnotations.ValidationException exc)
					{
						isValid = false;
						ModelState.AddModelError(property.Name, exc.Message);
					}
				}
			}
			return isValid;
		}

		private void FillEntity(object item, Entity entity)
		{
			var request = HttpContext.Current.Request;
			foreach (var property in entity.CreateProperties(false))
			{
				if (property.DataType == DataType.File)
				{
					var file = request.Files[property.Name];
					var fileName = String.Empty;
					if (property.ImageOptions.NameCreation == NameCreation.UserInput)
					{
						fileName = "test";
					}
					FileUpload.FileUpload.SaveImage(file, property.ImageOptions.MaxFileSize, property.ImageOptions.AllowedFileExtensions, out fileName, property.ImageOptions.NameCreation, property.ImageOptions.Settings.ToArray());

					property.Value = fileName;
				}

				var propertyInfo = entity.Type.GetProperty(property.Name);
				propertyInfo.SetValue(item, property.Value, null);
			}
		}

		public void FillEntity(Entity entity, FormCollection collection)
		{
			foreach (var property in entity.Properties)
			{
				var value = collection.GetValue(property.Name);
				if (value != null)
				{
					if (property.IsForeignKey && property.IsCollection)
					{
						property.Values = value.AttemptedValue.Split(",".ToCharArray()).OfType<object>().ToList();
					}
					else
					{
						property.Value = value.ConvertTo(property.PropertyType, CultureInfo.InvariantCulture);
					}
				}
			}
		}

		public void FillEntity(Entity entity, string key)
		{
			var item = GetEntity(entity, key);
			if (item == null)
			{
				Error(IlaroAdminResources.EntityNotExist);
				return;
			}

			var propertiesDict = item as IDictionary<string, object>;

			foreach (var property in entity.CreateProperties(false))
			{
				property.Value = propertiesDict.ContainsKey(property.ColumnName) ? propertiesDict[property.ColumnName] : null;
			}

			entity.Key.Value = key;
		}

		private object GetEntity(Entity entity, string key)
		{
			var keyObject = GetKeyObject(entity, key);

			return GetEntity(entity, keyObject);
		}

		private object GetEntity(Entity entity, object key)
		{
			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.ColumnName);

			var result = table.Single(key);

			return result;
		}

		private object GetKeyObject(Entity entity, string key)
		{
			var keyType = entity.Key.PropertyType;
			if (keyType.In(typeof(int), typeof(short), typeof(long)))
			{
				return long.Parse(key);
			}
			else if (keyType.In(typeof(Guid)))
			{
				return Guid.Parse(key);
			}
			else
			{
				return key;
			}
		}

		public bool Delete(Entity entity, string key, IList<PropertyDeleteViewModel> propertiesDeleteOptions)
		{
			var deleteOptions = propertiesDeleteOptions.ToDictionary(x => x.PropertyName, x => x.DeleteOption);
			foreach (var property in entity.Properties.Where(x => x.IsForeignKey && x.IsCollection))
			{
				if (!deleteOptions.ContainsKey(property.ForeignEntity.Name))
				{
					deleteOptions[property.ForeignEntity.Name] = property.DeleteOption;
				}
			}

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.ColumnName);

			var keyObject = GetKeyObject(entity, key);
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
				Error(IlaroAdminResources.EntityNotExist);
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

		public IList<IEntityFilter> PrepareFilters(Entity entity, HttpRequestBase request)
		{
			var filters = new List<IEntityFilter>();

			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.Bool))
			{
				var value = request[property.Name];

				var filter = new BoolEntityFilter();
				filter.Initialize(property, value);
				filters.Add(filter);
			}

			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.Enum))
			{
				var value = request[property.Name];

				var filter = new EnumEntityFilter();
				filter.Initialize(property, value);
				filters.Add(filter);
			}

			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.DateTime))
			{
				var value = request[property.Name];

				var filter = new DateTimeEntityFilter();
				filter.Initialize(property, value);
				filters.Add(filter);
			}

			return filters;
		}

		public object GetKeyValue(Entity entity, object savedItem)
		{
			return ((dynamic)savedItem).ID;
		}

		public IList<GroupPropertiesViewModel> PrepareGroups(Entity entity, bool getKey = true, string key = null)
		{
			var properties = entity.CreateProperties(getKey);
			foreach (var foreign in properties.Where(x => x.IsForeignKey))
			{
				var records = GetRecords(foreign.ForeignEntity, determineDisplayValue: true);
				foreign.PossibleValues = records.ToDictionary(x => x.KeyValue, x => x.DisplayName);
				if (foreign.IsCollection)
				{
					foreign.Values = records.Where(x => x.Values.Any(y => y.Property.ForeignEntity == entity && y.Value == key)).Select(x => x.KeyValue).OfType<object>().ToList();
				}
			}
			//var groupsDict = properties.GroupBy(x => x.GroupName).ToDictionary(x => x.Key);

			//var groups = new List<GroupPropertiesViewModel>();
			//if (entity.Groups.IsNullOrEmpty())
			//{
			//	foreach (var group in groupsDict)
			//	{
			//		groups.Add(new GroupPropertiesViewModel
			//		{
			//			GroupName = group.Key,
			//			Properties = group.Value.ToList()
			//		});
			//	}
			//}
			//else
			//{
			//	foreach (var groupName in entity.Groups)
			//	{
			//		var trimedGroupName = groupName.TrimEnd('*');
			//		if (groupsDict.ContainsKey(trimedGroupName ?? Resources.IlaroAdminResources.Others))
			//		{
			//			var group = groupsDict[trimedGroupName];

			//			groups.Add(new GroupPropertiesViewModel
			//			{
			//				GroupName = group.Key,
			//				Properties = group.ToList(),
			//				IsCollapsed = groupName.EndsWith("*")
			//			});
			//		}
			//	}
			//}

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
			var records = model.Query(sql, entity.Key.Value);

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
			var rowData = ExpandoToDataRow(baseRecord, hierarchy.Entity, prefix);

			var recordHierarchy = new RecordHierarchy
			{
				Entity = hierarchy.Entity,
				KeyValue = rowData.KeyValue,
				DisplayName = GetDisplayName(hierarchy.Entity, rowData),
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
					var rowData = ExpandoToDataRow(recordDict, hierarchy.Entity, prefix);

					var subRecord = new RecordHierarchy
					{
						Entity = hierarchy.Entity,
						KeyValue = rowData.KeyValue,
						DisplayName = GetDisplayName(hierarchy.Entity, rowData),
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

			var columns = flatHierarchy.SelectMany(x => x.Entity.DisplayProperties
				.Select(y => x.Alias + "." + y.ColumnName + " AS " + x.Alias.Trim("[]".ToCharArray()) + "_" + y.ColumnName)).ToList();
			var joins = new List<string>();
			foreach (var item in flatHierarchy.Where(x => x.ParentHierarchy != null))
			{
				var foreignTable = item.Entity.TableName;
				var foreignAlias = item.Alias;
				var foreignKey = String.Empty;
				var baseTableAlias = item.ParentHierarchy.Alias;
				var baseTablePrimaryKey = String.Empty;
				var foreignProperty = item.Entity.Properties.FirstOrDefault(x => x.ForeignEntity == item.ParentHierarchy.Entity);
				if (foreignProperty == null || foreignProperty.IsCollection)
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

			foreach (var property in entity.CreateProperties().Where(x => x.IsForeignKey && x.IsCollection))
			{
				if (parent == null || (parent != null && parent.Entity != property.ForeignEntity))
				{
					index++;
					hierarchy.SubHierarchies.Add(GetEntityHierarchy(hierarchy, property.ForeignEntity, ref index));
				}
			}

			return hierarchy;
		}
	}
}