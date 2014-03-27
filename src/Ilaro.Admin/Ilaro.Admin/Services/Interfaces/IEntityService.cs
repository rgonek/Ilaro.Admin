using Ilaro.Admin.EntitiesFilters;
using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.Services.Interfaces
{
	public interface IEntityService
	{
		void ClearProperties(EntityViewModel entity);

		object Create(EntityViewModel entity);

		bool Delete(EntityViewModel entity, string key);

		object Edit(EntityViewModel entity);

		void FillEntity(EntityViewModel entity, string key);

		void FillEntity(EntityViewModel entity, FormCollection collection);

		IList<Ilaro.Admin.ViewModels.DataRowViewModel> GetData(EntityViewModel entity);

		IList<DataRowViewModel> GetData(EntityViewModel entity, int page, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection);

		IList<DataRowViewModel> GetData(EntityViewModel entity, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection);

		object GetKeyValue(EntityViewModel entity, object savedItem);

		string GetTableName(string sql);

		IList<ColumnViewModel> PrepareColumns(EntityViewModel entity, string order, string orderDirection);

		IList<IEntityFilter> PrepareFilters(EntityViewModel entity, HttpRequestBase request);

		IList<GroupPropertiesViewModel> PrepareGroups(EntityViewModel entity, bool getKey = true);

		int TotalItems(EntityViewModel entity, IList<IEntityFilter> filters, string searchQuery);

		bool ValidateEntity(EntityViewModel entity, ModelStateDictionary ModelState);
	}
}