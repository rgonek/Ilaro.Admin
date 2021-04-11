using Ilaro.Admin.Core.Models;
using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordDeleter
    {
        bool Delete(
            EntityRecord entityRecord, 
            IDictionary<string, PropertyDeleteOption> options, 
            Func<string> changeDescriber = null);
    }
}