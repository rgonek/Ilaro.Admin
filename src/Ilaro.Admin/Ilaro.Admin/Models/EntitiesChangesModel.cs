using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class EntitiesChangesModel : EntitiesIndexModel
    {
        public Entity EntityChangesFor { get; set; }

        public string Key { get; set; }

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
