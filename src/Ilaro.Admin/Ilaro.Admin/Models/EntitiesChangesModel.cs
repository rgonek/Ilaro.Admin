using Ilaro.Admin.Core;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Models
{
    public class EntitiesChangesModel : EntitiesIndexModel
    {
        public Entity EntityChangesFor { get; set; }

        public string Key { get; set; }

        public IEnumerable<ChangeRow> ChangeData
        {
            get
            {
                return Data.Select(x => new ChangeRow(x));
            }
        }

        public EntitiesChangesModel(
            Entity entity,
            PagedRecords pagedRecords,
            TableInfo tableInfo,
            string url)
            : base(entity, pagedRecords, tableInfo, url)
        {
        }
    }
}
