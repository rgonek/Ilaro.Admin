using System.Collections.Generic;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core.Data
{
    public interface IDeletingRecords
    {
        bool Delete(Entity entity, IDictionary<string, DeleteOption> options);
    }
}