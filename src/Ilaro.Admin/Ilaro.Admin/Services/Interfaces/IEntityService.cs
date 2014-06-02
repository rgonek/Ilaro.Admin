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
		PagedRecordsViewModel GetRecords(Entity entity, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection);

		/// <summary>
		/// Get list of changes for entity
		/// </summary>
		/// <param name="entity">Entity info</param>
		PagedRecordsViewModel GetChangesRecords(Entity entityChangesFor, int page, int take, IList<IEntityFilter> filters, string searchQuery, string order, string orderDirection);

		/// <summary>
		/// Clear properties values, for example after edit, or before display add form
		/// </summary>
		/// <param name="entity">Entity info</param>
		void ClearProperties(Entity entity);

		object Create(Entity entity);

		/// <summary>
		/// Delete entity
		/// </summary>
		/// <param name="entity">Entity info</param>
		bool Delete(Entity entity, string key, IList<PropertyDeleteViewModel> propertiesDeleteOptions);

		int Edit(Entity entity);

		void FillEntity(Entity entity, string key);

		/// <summary>
		/// Fill entity properties values with posted form data
		/// </summary>
		/// <param name="entity">Entity info</param>
		/// <param name="collection">Form collection</param>
		void FillEntity(Entity entity, FormCollection collection);

		object GetKeyValue(Entity entity, object savedItem);

		IList<ColumnViewModel> PrepareColumns(Entity entity, string order, string orderDirection);

		IList<IEntityFilter> PrepareFilters(Entity entity, HttpRequestBase request);

		IList<GroupPropertiesViewModel> PrepareGroups(Entity entity, bool getKey = true, string key = null);

		/// <summary>
		/// Validate entity
		/// </summary>
		/// <param name="entity">Entity info</param>
		bool ValidateEntity(Entity entity, ModelStateDictionary ModelState);

		RecordHierarchy GetRecordHierarchy(Entity entity);
	}
}