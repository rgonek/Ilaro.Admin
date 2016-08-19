using Ilaro.Admin.Core;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core.Models
{
    public class EntitiesChangesModel : EntitiesIndexModel
    {
        public Entity EntityChangesFor { get; set; }

        public string Key { get; set; }

        public IEnumerable<ChangeRow> ChangeData
        {
            get
            {
                return Records.Select(x => new ChangeRow(x));
            }
        }

        public EntitiesChangesModel(
            Entity entity,
            PagedRecords pagedRecords,
            TableInfo tableInfo)
            : base(entity, pagedRecords, tableInfo)
        {
        }
    }
}
