using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ilaro.Admin.ViewComponents
{
    public sealed class Breadcrumb : ViewComponent
    {
        private readonly IEntityCollection _entities;

        public Breadcrumb(IEntityCollection entities)
            => _entities = entities;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var entityName = RouteData.Values[nameof(Entity).ToLower()];
            var activePage = RouteData.Values["page"].ToString().Trim('/');
            var breadcrumbItems = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Home", "Index", activePage)
            };

            if (entityName != null)
            {
                var entity = _entities[entityName.ToString()];
                var listPage = new BreadcrumbItem(entity.Verbose.Plural, "List", entity.Name, activePage);
                breadcrumbItems.Add(listPage);

                if (listPage.IsActive == false)
                {
                    breadcrumbItems.Add(new BreadcrumbItem(activePage + " " + entity.Verbose.Singular, activePage, true));
                }
            }

            await Task.CompletedTask;
            return View(breadcrumbItems);
        }
    }
}
