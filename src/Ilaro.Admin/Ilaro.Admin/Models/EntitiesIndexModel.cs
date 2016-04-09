using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core;
using Ilaro.Admin.Filters;

namespace Ilaro.Admin.Models
{
    public class EntitiesIndexModel
    {
        public EntitiesIndexModel(
            Entity entity,
            PagedRecords pagedRecords,
            TableInfo tableInfo,
            string url)
        {
            Data = pagedRecords.Records;
            Columns = entity.DisplayProperties
                .Select(x => new Column(x, tableInfo.Order, tableInfo.OrderDirection)).ToList();
            Entity = entity;
            Pager =
                new PagerInfo(url, tableInfo.PerPage, tableInfo.Page, pagedRecords.TotalItems);
            Filters = pagedRecords.Filters.Where(x => x.DisplayInUI).ToList();
            TableInfo = tableInfo;
        }

        public Entity Entity { get; set; }

        public IList<DataRow> Data { get; set; }

        public IList<Column> Columns { get; set; }

        public PagerInfo Pager { get; set; }

        public IList<BaseFilter> Filters { get; set; }

        public IList<BaseFilter> ActiveFilters
        {
            get
            {
                return Filters.Where(x => x.IsActive).ToList();
            }
        }

        public TableInfo TableInfo { get; set; }

        public IConfiguration Configuration { get; set; }

        public bool ChangeEnabled { get; set; }
    }
}
