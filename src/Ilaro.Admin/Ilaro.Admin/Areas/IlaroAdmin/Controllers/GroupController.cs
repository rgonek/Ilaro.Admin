using System;
using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;
using Ilaro.Admin.DataAnnotations;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    [AuthorizeWrapper]
    public class GroupController : Controller
    {
        private readonly Notificator _notificator;
        private readonly IIlaroAdmin _admin;

        public GroupController(IIlaroAdmin admin ,Notificator notificator)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));

            _admin = admin;
            _notificator = notificator;
        }

        public virtual ActionResult Index()
        {
            var model = new GroupIndexModel
            {
                Groups = _admin.Entities
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
            var model = _admin.Entities
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
