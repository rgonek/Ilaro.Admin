using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IDeletingRecords
    {
        bool Delete(Entity entity, IDictionary<string, DeleteOption> options);
    }
}