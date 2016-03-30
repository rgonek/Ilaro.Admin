using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IDeletingRecords
    {
        bool Delete(
            EntityRecord entityRecord, 
            IDictionary<string, DeleteOption> options, 
            Func<string> changeDescriber = null);
    }
}