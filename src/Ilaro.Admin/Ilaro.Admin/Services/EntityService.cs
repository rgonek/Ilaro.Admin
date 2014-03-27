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

namespace Ilaro.Admin.Services
{
	public class EntityService : BaseService, IEntityService
	{
		public EntityService(Notificator notificator)
			: base(notificator)
		{
		}

		public IList<DataRowViewModel> GetData(EntityViewModel entity)
		{
			var context = AdminInitialize.Context;
			var sql = context.Set(entity.Type).ToString();

			return GetData(entity, context, sql);
		}

		public IList<DataRowViewModel> GetData(EntityViewModel entity, int page, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
		{
			return GetData(entity, page, Consts.ItemsQuantityPerPage, filters, searchQuery, order, orderDirection);
		}

		public IList<DataRowViewModel> GetData(EntityViewModel entity, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection)
		{
			var skip = (page - 1) * take;

			var context = AdminInitialize.Context;
			var search = new EntitySearch { Query = searchQuery, Properties = entity.SearchProperties };
			order = order.IsNullOrEmpty() ? entity.Key.Name : order;
			orderDirection = orderDirection.IsNullOrEmpty() ? "ASC" : orderDirection.ToUpper();
			var sql = PreparePagingSQL(context.Set(entity.Type).ToString(), order, orderDirection, skip, take, filters, search);

			return GetData(entity, context, sql);
		}

		private IList<DataRowViewModel> GetData(EntityViewModel entity, DbContext context, string sql)
		{
			var data = new List<DataRowViewModel>();

			using (var cn = new SqlConnection(context.Database.Connection.ConnectionString))
			{
				cn.Open();
				using (var cm = new SqlCommand(sql, cn))
				{
					using (var rd = cm.ExecuteReader())
					{
						while (rd.Read())
						{
							var row = new DataRowViewModel();
							row.KeyValue = GetValue(rd, entity.Key.Name);
							row.LinkKeyValue = GetValue(rd, entity.LinkKey.Name);
							foreach (var property in entity.DisplayColumns)
							{
								row.Values.Add(new CellValueViewModel
								{
									Value = GetValue(rd, property.Name),
									Property = property
								});
							}
							data.Add(row);
						}
					}
				}
				cn.Close();
			}

			return data;
		}

		private string PreparePagingSQL(string sql, string order, string orderDirection, int skip, int take, IList<IEntityFilter> filters, EntitySearch search)
		{
			var selectText = "SELECT";
			var start = sql.IndexOf(selectText) + selectText.Length + 1;
			var end = sql.IndexOf("FROM", start);

			var toSelect = sql.Substring(start, end - start);
			var alias = toSelect.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

			sql = sql.Replace(toSelect, string.Format("{0}, row_number() OVER (ORDER BY {1}.{2} {3}) AS [row_number]", toSelect, alias, order, orderDirection));

			var selectFormat = @"SELECT TOP ({6}) 
{1}
FROM ( {0}
{7})  AS {2}
WHERE {2}.[row_number] > {5}
ORDER BY {2}.[{3}] {4}";

			return string.Format(selectFormat, sql, toSelect, alias, order, orderDirection, skip, take, ConvertFiltersToSQL(filters, search, alias));
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

			return " WHERE" + conditions.TrimEnd("AND".ToCharArray()) + " ";
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

		private string GetValue(IDataReader reader, string name)
		{
			var value = reader[name];
			if (value == System.DBNull.Value)
			{
				return null;
			}
			else
			{
				return value.ToString();
			}
		}

		public int TotalItems(EntityViewModel entity, IList<IEntityFilter> filters, string searchQuery)
		{
			var context = AdminInitialize.Context;
			var tableName = GetTableName(context.Set(entity.Type).ToString());
			var search = new EntitySearch { Query = searchQuery, Properties = entity.SearchProperties };
			var sql = string.Format("SELECT count(*) FROM [dbo].{0}{1}", tableName, ConvertFiltersToSQL(filters, search));

			int totalItems = 0;

			using (var cn = new SqlConnection(context.Database.Connection.ConnectionString))
			{
				cn.Open();
				using (var cm = new SqlCommand(sql, cn))
				{
					totalItems = (int)cm.ExecuteScalar();
				}
				cn.Close();
			}

			return totalItems;
		}

		public string GetTableName(string sql)
		{
			var fromText = "FROM [dbo].";
			var start = sql.IndexOf(fromText) + fromText.Length;
			var end = sql.IndexOf("]", start) + 1;

			return sql.Substring(start, end - start);
		}

		public object Create(EntityViewModel entity)
		{
			var context = AdminInitialize.Context;

			var existingItem = context.Set(entity.Type).Find(entity.Key.Value);
			if (existingItem != null)
			{
				Error("Already exist");
				return null;
			}

			var item = context.Set(entity.Type).Create(entity.Type);

			FillEntity(item, entity);

			context.Set(entity.Type).Add(item);
			context.SaveChanges();

			ClearProperties(entity);

			return item;
		}

		public object Edit(EntityViewModel entity)
		{
			var context = AdminInitialize.Context;

			if (entity.Key.Value == null)
			{
				Error("Key is null");
				return null;
			}

			var existingItem = GetEntity(context, entity, entity.Key.Value.ToString());
			if (existingItem == null)
			{
				Error("Not exist");
				return null;
			}

			FillEntity(existingItem, entity);

			context.SaveChanges();

			ClearProperties(entity);

			return existingItem;
		}

		public void ClearProperties(EntityViewModel entity)
		{
			foreach (var property in entity.Properties)
			{
				property.Value = null;
			}
		}

		public bool ValidateEntity(EntityViewModel entity, ModelStateDictionary ModelState)
		{
			var request = HttpContext.Current.Request;
			foreach (var property in entity.Properties.Where(x => x.DataType == DataType.File))
			{
				var file = request.Files["field." + property.Name];
				var result = FileUpload.Validate(file, property.ImageOptions.MaxFileSize, property.ImageOptions.AllowedFileExtensions, !property.IsRequired);

				if (result != FileUploadValidationResult.Valid)
				{
					return false;
				}
			}
			return true;
		}

		private void FillEntity(object item, EntityViewModel entity)
		{
			var request = HttpContext.Current.Request;
			foreach (var property in entity.CreateProperties())
			{
				if (property.DataType == DataType.File)
				{
					var file = request.Files["field." + property.Name];
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
				var value = collection.GetValue("field." + property.Name);
				if (value != null)
				{
					property.Value = value.ConvertTo(property.PropertyType);
				}
			}
		}

		public void FillEntity(EntityViewModel entity, string key)
		{
			var context = AdminInitialize.Context;

			var item = GetEntity(context, entity, key);
			if (item == null)
			{
				Error("Not exist");
				return;
			}

			foreach (var property in entity.CreateProperties(false))
			{
				var propertyInfo = entity.Type.GetProperty(property.Name);
				property.Value = propertyInfo.GetValue(item, null);
			}

			entity.Key.Value = key;
		}

		private object GetEntity(DbContext context, EntityViewModel entity, string key)
		{
			var keyType = entity.Key.PropertyType;
			if (keyType.In(typeof(int), typeof(short), typeof(long)))
			{
				return context.Set(entity.Type).Find(long.Parse(key));
			}
			else if (keyType.In(typeof(Guid)))
			{
				return context.Set(entity.Type).Find(Guid.Parse(key));
			}
			else
			{
				return context.Set(entity.Type).Find(key);
			}
		}

		public bool Delete(EntityViewModel entity, string key)
		{
			var context = AdminInitialize.Context;

			var item = GetEntity(context, entity, key);
			if (item == null)
			{
				Error("Not exist");
				return false;
			}

			context.Set(entity.Type).Remove(item);
			context.SaveChanges();

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
			var propertyInfo = entity.Type.GetProperty(entity.Key.Name);
			return propertyInfo.GetValue(savedItem, null);
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
					if (groupsDict.ContainsKey(trimedGroupName ?? "Pozostałe"))
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
	}
}