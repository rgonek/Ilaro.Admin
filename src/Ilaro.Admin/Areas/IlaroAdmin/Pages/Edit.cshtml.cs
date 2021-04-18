using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ilaro.Admin.Areas.IlaroAdmin.Pages
{
    public class EditModel : PageModel
    {
        private readonly IEntityService _entityService;
        private readonly IRecordFetcher _recordFetcher;

        public Entity Entity { get; private set; }

        public EntityRecord Record { get; private set; }

        public IList<GroupProperties> PropertiesGroups { get; private set; }

        public object ConcurrencyCheck { get; private set; }

        public EditModel(IEntityService entityService, IRecordFetcher recordFetcher)
        {
            _entityService = entityService;
            _recordFetcher = recordFetcher;
        }

        public void OnGet(Entity entity, string id)
        {
            Entity = entity;
            Record = _recordFetcher.GetEntityRecord(entity, id);
            PropertiesGroups = _entityService.PrepareGroups(Record, getKey: false, key: id);
            ConcurrencyCheck = Record.GetConcurrencyCheckValue();
        }

        public IActionResult OnPost(
            Entity entity,
            string id,
            [Bind(Prefix = "__ConcurrencyCheck")] string concurrencyCheck,
            IFormCollection collection)
        {
            var concurrencyCheckValue = Core.ConcurrencyCheck.Convert(concurrencyCheck, entity);

            var isSuccess = _entityService.Edit(entity, id, collection, concurrencyCheckValue);

            return RedirectToPage("List", new { entity = entity.Name });
        }
    }
}
