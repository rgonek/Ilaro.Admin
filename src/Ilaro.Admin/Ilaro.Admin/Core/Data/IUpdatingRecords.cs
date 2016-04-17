using System;
namespace Ilaro.Admin.Core.Data
{
    public interface IUpdatingRecords
    {
        bool Update(
            EntityRecord entityRecord, 
            object concurrencyCheckValue = null, 
            Func<string> changeDecriber = null);
    }
}