using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ilaro.Admin.Areas.IlaroAdmin.Pages
{
    public class CreateModel : PageModel
    {
        private readonly IEntityService _entityService;

        public Entity Entity { get; private set; }

        public IList<GroupProperties> PropertiesGroups { get; private set; }

        public CreateModel(IEntityService entityService)
        {
            _entityService = entityService;
        }

        public void OnGet(Entity entity)
        {
            Entity = entity;
            PropertiesGroups = _entityService.PrepareGroups(entity.CreateEmptyRecord());
        }

        public IActionResult OnPost(Entity entity, IFormCollection collection)
        {
            var savedId = _entityService.Create(entity, collection);

            return RedirectToPage("List", new { entity = entity.Name });
        }
    }
}
