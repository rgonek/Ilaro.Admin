using System;
using System.Collections.Generic;

namespace Ilaro.Admin.DataAccess
{
    public interface IRecordDeleter
    {
        bool Delete(
            EntityRecord entityRecord,
            IDictionary<string, PropertyDeleteOption> options,
            Func<string> changeDescriber = null);
    }
}