using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class GroupController : BaseController
    {
        public GroupController(Notificator notificator)
            : base(notificator)
        {
        }

        public virtual ActionResult Index()
        {
            var model = new GroupIndexModel
            {
                Groups = AdminInitialise.EntitiesTypes
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
            var model = AdminInitialise.EntitiesTypes
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
