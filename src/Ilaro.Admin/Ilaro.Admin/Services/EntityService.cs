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
using Ilaro.Admin.Commons.FileUpload;
using Ilaro.Admin.Commons.Notificator;
using System.Data.Common;
using Massive;
using System.Dynamic;
using System.Collections.Specialized;
using Resources;
using Ilaro.Admin.Model;

namespace Ilaro.Admin.Services
{
	public class EntityService : BaseService, IEntityService
	{
		public EntityService(Notificator notificator)
			: base(notificator)
		{
		}

		public PagedRecordsViewModel GetRecords(EntityViewModel entity, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
		{
			var skip = (page - 1) * take;

			var search = new EntitySearch { Query = searchQuery, Properties = entity.SearchProperties };
			order = order.IsNullOrEmpty() ? entity.Key.Name : order;
			orderDirection = orderDirection.IsNullOrEmpty() ? "ASC" : orderDirection.ToUpper();
			var orderBy = order + " " + orderDirection;
			var columns = string.Join(",", entity.DisplayColumns.Select(x => x.Name));
			var where = ConvertFiltersToSQL(filters, search);

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var result = table.Paged(columns: columns, where: where, orderBy: orderBy, currentPage: page, pageSize: take);

			var data = new List<DataRowViewModel>();
			foreach (var item in result.Items)
			{
				var dict = (IDictionary<String, Object>)item;
				var row = new DataRowViewModel();
				row.KeyValue = dict[entity.Key.Name].ToStringSafe();
				row.LinkKeyValue = dict[entity.LinkKey.Name].ToStringSafe();
				foreach (var property in entity.DisplayColumns)
				{
					row.Values.Add(new CellValueViewModel
					{
						Value = dict[property.Name].ToStringSafe(),
						Property = property
					});
				}
				data.Add(row);
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

			return " WHERE" + conditions.TrimEnd("AND".ToCharArray());
		}

		public IList<ColumnViewModel> PrepareColumns(EntityViewModel entity, string order, string orderDirection)
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

			return entity.DisplayColumns.Select(x => new ColumnViewModel
			{
				Name = x.Name,
				DisplayName = x.DisplayName,
				Description = x.Description,
				SortDirection = x.Name.ToLower() == order ? orderDirection : String.Empty
			}).ToList();
		}

		public object Create(EntityViewModel entity)
		{
			var existingItem = GetEntity(entity, entity.Key.Value);
			if (existingItem != null)
			{
				Error(IlaroAdminResources.EntityAlreadyExist);
				return null;
			}

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var expando = new ExpandoObject();
			var filler = expando as IDictionary<String, object>;
			foreach (var property in entity.Properties.Where(x => !x.IsKey && !x.IsForeignKey))
			{
				filler[property.Name] = property.Value;
			}
			var item = table.Insert(expando);

			AddEntityChange(entity.Name, ((object)item.ID).ToStringSafe(), EntityChangeType.Insert);

			ClearProperties(entity);

			return item;
		}

		//private object Get

		public object Edit(EntityViewModel entity)
		{
			if (entity.Key.Value == null)
			{
				Error(IlaroAdminResources.EntityKeyIsNull);
				return null;
			}

			var existingItem = GetEntity(entity, entity.Key.Value);
			if (existingItem == null)
			{
				Error(IlaroAdminResources.EntityNotExist);
				return null;
			}

			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var expando = new ExpandoObject();
			var filler = expando as IDictionary<String, object>;
			foreach (var property in entity.Properties.Where(x => !x.IsKey && !x.IsForeignKey))
			{
				filler[property.Name] = property.Value;
			}
			var savedItem = table.Update(expando, entity.Key.Value);

			// TODO: get info about changed properties
			AddEntityChange(entity.Name, entity.Key.StringValue, EntityChangeType.Update);

			ClearProperties(entity);

			return savedItem;
		}

		/// <summary>
		/// Clear properties values
		/// </summary>
		public void ClearProperties(EntityViewModel entity)
		{
			foreach (var property in entity.Properties)
			{
				property.Value = null;
			}
		}

		public bool ValidateEntity(EntityViewModel entity, ModelStateDictionary ModelState)
		{
			bool isValid = true;
			var request = HttpContext.Current.Request;
			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.File))
			{
				var file = request.Files[property.Name];
				var result = FileUpload.Validate(file, property.ImageOptions.MaxFileSize, property.ImageOptions.AllowedFileExtensions, !property.IsRequired);

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

		private void FillEntity(object item, EntityViewModel entity)
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
					FileUpload.SaveImage(file, property.ImageOptions.MaxFileSize, property.ImageOptions.AllowedFileExtensions, out fileName, property.ImageOptions.NameCreation, property.ImageOptions.Settings);

					property.Value = fileName;
				}

				var propertyInfo = entity.Type.GetProperty(property.Name);
				propertyInfo.SetValue(item, property.Value, null);
			}
		}

		public void FillEntity(EntityViewModel entity, FormCollection collection)
		{
			foreach (var property in entity.Properties)
			{
				var value = collection.GetValue(property.Name);
				if (value != null)
				{
					property.Value = value.ConvertTo(property.PropertyType);
				}
			}
		}

		public void FillEntity(EntityViewModel entity, string key)
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
				property.Value = propertiesDict.ContainsKey(property.Name) ? propertiesDict[property.Name] : null;
			}

			entity.Key.Value = key;
		}

		private object GetEntity(EntityViewModel entity, string key)
		{
			var keyObject = GetKeyObject(entity, key);

			return GetEntity(entity, keyObject);
		}

		private object GetEntity(EntityViewModel entity, object key)
		{
			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var result = table.Single(key);

			return result;
		}

		private object GetKeyObject(EntityViewModel entity, string key)
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

		public bool Delete(EntityViewModel entity, string key)
		{
			var table = new DynamicModel(AdminInitialise.ConnectionString, tableName: entity.TableName, primaryKeyField: entity.Key.Name);

			var keyObject = GetKeyObject(entity, key);

			var result = table.Delete(keyObject);

			if (result < 1)
			{
				Error(IlaroAdminResources.EntityNotExist);
				return false;
			}

			AddEntityChange(entity.Name, key, EntityChangeType.Delete);

			return true;
		}

		public IList<IEntityFilter> PrepareFilters(EntityViewModel entity, HttpRequestBase request)
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

		public object GetKeyValue(EntityViewModel entity, object savedItem)
		{
			return ((dynamic)savedItem).ID;
		}

		public IList<GroupPropertiesViewModel> PrepareGroups(EntityViewModel entity, bool getKey = true)
		{
			var groupsDict = entity.CreateProperties(getKey).GroupBy(x => x.GroupName).ToDictionary(x => x.Key);

			var groups = new List<GroupPropertiesViewModel>();
			if (entity.Groups.IsNullOrEmpty())
			{
				foreach (var group in groupsDict)
				{
					groups.Add(new GroupPropertiesViewModel
					{
						GroupName = group.Key,
						Properties = group.Value.ToList()
					});
				}
			}
			else
			{
				foreach (var groupName in entity.Groups)
				{
					var trimedGroupName = groupName.TrimEnd('*');
					if (groupsDict.ContainsKey(trimedGroupName ?? "Others"))
					{
						var group = groupsDict[trimedGroupName];

						groups.Add(new GroupPropertiesViewModel
						{
							GroupName = group.Key,
							Properties = group.ToList(),
							IsCollapsed = groupName.EndsWith("*")
						});
					}
				}
			}

			return groups;
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
	}
}