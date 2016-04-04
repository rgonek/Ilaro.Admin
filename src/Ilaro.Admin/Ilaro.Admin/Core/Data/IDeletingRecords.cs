using Ilaro.Admin.Models;
using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.Data
{
    public interface IDeletingRecords
    {
        bool Delete(
            EntityRecord entityRecord, 
            IDictionary<string, PropertyDeleteOption> options, 
            Func<string> changeDescriber = null);
    }
}