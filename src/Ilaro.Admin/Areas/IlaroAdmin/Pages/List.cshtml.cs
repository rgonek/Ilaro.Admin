using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ilaro.Admin.Areas.IlaroAdmin.Pages
{
    public class ListModel : PageModel
    {
        public Entity Entity { get; private set; }

        public ListModel()
        {
        }

        public void OnGet(Entity entity, [FromQuery] TableInfo tableInfo)
        {
            //Entity = _entities[name];

            //Request.
        }
    }
}
