using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Ilaro.Admin.ViewComponents
{
    public sealed class Sidebar : ViewComponent
    {
        private readonly IEntityCollection _entities;

        public Sidebar(IEntityCollection entities)
            => _entities = entities;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var groups = _entities.GroupBy(x => x.Verbose.Group)
                .Select(x => new GroupModel(x.Key, x.ToList()))
                .ToList();

            await Task.CompletedTask;
            return View(groups);
        }
    }
}
