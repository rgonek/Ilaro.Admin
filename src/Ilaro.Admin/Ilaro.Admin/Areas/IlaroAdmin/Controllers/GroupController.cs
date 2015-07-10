using System;
using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class GroupController : Controller
    {
        private readonly Notificator _notificator;

        public GroupController(Notificator notificator)
        {
            if (notificator == null)
                throw new ArgumentNullException("notificator");

            _notificator = notificator;
        }

        public virtual ActionResult Index()
        {
            var model = new GroupIndexModel
            {
                Groups = Admin.EntitiesTypes
                    .GroupBy(x => x.Verbose.Group)
                    .Select(x => new GroupModel
                    {
                        Name = x.Key,
                        Entities = x.ToList()
                    }).ToList()
            };

            return View(model);
        }

        public virtual ActionResult Details(string groupName)
        {
            var model = Admin.EntitiesTypes
                .GroupBy(x => x.Verbose.Group)
                .Where(x => x.Key == groupName)
                .Select(x => new GroupModel
                {
                    Name = x.Key,
                    Entities = x.ToList()
                }).FirstOrDefault();

            return View(model);
        }
    }
}
