using Ilaro.Admin.Core;
using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Areas.IlaroAdmin.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly IEntityService _entityService;
        private readonly IRecordFetcher _recordFetcher;
        private readonly IRecordHierarchyFetcher _recordHierarchyFetcher;

        public Entity Entity { get; private set; }

        public EntityRecord Record { get; private set; }

        public RecordHierarchy RecordHierarchy { get; private set; }

        public IList<PropertyDeleteOption> PropertiesDeleteOptions { get; private set; }

        public bool DisplayRecordHierarchy { get; private set; }
        public bool AssumableDeleteHierarchyWarning { get; private set; }

        public DeleteModel(IEntityService entityService, IRecordFetcher recordFetcher, IRecordHierarchyFetcher recordHierarchyFetcher)
        {
            _entityService = entityService;
            _recordFetcher = recordFetcher;
            _recordHierarchyFetcher = recordHierarchyFetcher;
        }

        public void OnGet(Entity entity, IdValue id)
        {
            Entity = entity;
            Record = _recordFetcher.GetEntityRecord(entity, id);
            var deleteOptions = DeleteOptionsHierarchyBuilder.GetHierarchy(entity);
            RecordHierarchy = _recordHierarchyFetcher.GetRecordHierarchy(Record, deleteOptions);

            DisplayRecordHierarchy = deleteOptions.Any();
            AssumableDeleteHierarchyWarning = deleteOptions.Any(x => x.ShowOptions);
            PropertiesDeleteOptions = deleteOptions.Any(x => x.DeleteOption == CascadeOption.AskUser)
                ? deleteOptions.Where(x => x.Visible).ToList()
                : new List<PropertyDeleteOption>();
        }

        public IActionResult OnPost(Entity entity, IdValue id, IList<PropertyDeleteOption> propertiesDeleteOptions)
        {
            var deleteOptions = DeleteOptionsHierarchyBuilder.Merge(
                entity,
                propertiesDeleteOptions);

            var isSuccess = _entityService.Delete(entity, id, deleteOptions);

            return RedirectToPage("List", new { entity = entity.Name });
        }
    }
}
