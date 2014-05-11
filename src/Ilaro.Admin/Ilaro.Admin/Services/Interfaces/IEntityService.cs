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
		/// <summary>
		/// Get list of records for certain page
		/// </summary>
		/// <param name="entity">Entity info</param>
		PagedRecordsViewModel GetRecords(EntityViewModel entity, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection);

		/// <summary>
		/// Get list of changes for entity
		/// </summary>
		/// <param name="entity">Entity info</param>
		PagedRecordsViewModel GetChangesRecords(EntityViewModel entityChangesFor, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection);

		/// <summary>
		/// Clear properties values, for example after edit, or before display add form
		/// </summary>
		/// <param name="entity">Entity info</param>
		void ClearProperties(EntityViewModel entity);

		object Create(EntityViewModel entity);

		/// <summary>
		/// Delete entity
		/// </summary>
		/// <param name="entity">Entity info</param>
		bool Delete(EntityViewModel entity, string key, IList<PropertyDeleteViewModel> propertiesDeleteOptions);

		int Edit(EntityViewModel entity);

		void FillEntity(EntityViewModel entity, string key);

		/// <summary>
		/// Fill entity properties values with posted form data
		/// </summary>
		/// <param name="entity">Entity info</param>
		/// <param name="collection">Form collection</param>
		void FillEntity(EntityViewModel entity, FormCollection collection);

		object GetKeyValue(EntityViewModel entity, object savedItem);

		IList<ColumnViewModel> PrepareColumns(EntityViewModel entity, string order, string orderDirection);

		IList<IEntityFilter> PrepareFilters(EntityViewModel entity, HttpRequestBase request);

		IList<GroupPropertiesViewModel> PrepareGroups(EntityViewModel entity, bool getKey = true, string key = null);

		/// <summary>
		/// Validate entity
		/// </summary>
		/// <param name="entity">Entity info</param>
		bool ValidateEntity(EntityViewModel entity, ModelStateDictionary ModelState);

		RecordHierarchy GetRecordHierarchy(EntityViewModel entity);
	}
}