using Ilaro.Admin.Core;
using Ilaro.Admin.Core.DataAccess;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Areas.IlaroAdmin.Pages
{
    public class ListModel : PageModel
    {
        private readonly IRecordService _recordService;

        public ListModel(IRecordService recordService)
            => _recordService = recordService;

        public Entity Entity { get; private set; }

        public IList<Column> Columns { get; private set; }

        public IList<EntityRecord> Records { get; private set; }

        public void OnGet(Entity entity, [FromQuery] TableInfo tableInfo)
        {
            Entity = entity;
            var pagedRecords = _recordService.GetRecords(entity, null, tableInfo);
            if (pagedRecords.Records.IsNullOrEmpty() && tableInfo.Page > 1)
            {

            }
            Columns = entity.DisplayProperties.Select(x => new Column(x, tableInfo.Order, tableInfo.OrderDirection)).ToList();
            Records = pagedRecords.Records;
        }
    }
}
