using System;
using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Core.Data;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    [AuthorizeWrapper]
    public class GroupController : Controller
    {
        private readonly Notificator _notificator;
        private readonly IIlaroAdmin _admin;
        private readonly IRecordsService _recordsService;

        public GroupController(
            IIlaroAdmin admin, 
            Notificator notificator, 
            IRecordsService recordsService)
        {
            if (admin == null)
                throw new ArgumentNullException(nameof(admin));
            if (notificator == null)
                throw new ArgumentNullException(nameof(notificator));
            if (recordsService == null)
                throw new ArgumentNullException(nameof(recordsService));

            _admin = admin;
            _notificator = notificator;
            _recordsService = recordsService;
        }

        public virtual ActionResult Index()
        {
            var model = new GroupIndexModel
            {
                Groups = _admin.Entities
                    .Except(new[] { _admin.ChangeEntity })
                    .GroupBy(x => x.Verbose.Group)
                    .Select(x => new GroupModel
                    {
                        Name = x.Key,
                        Entities = x.ToList()
                    }).ToList(),
                ChangeEnabled = _admin.ChangeEntity != null,
                Changes = _recordsService.GetLastChanges(10)
            };

            return View(model);
        }

        public virtual ActionResult Details(string groupName)
        {
            var model = _admin.Entities
                .Except(new[] { _admin.ChangeEntity })
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
