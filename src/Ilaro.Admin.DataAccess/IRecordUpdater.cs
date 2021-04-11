using System;
namespace Ilaro.Admin.DataAccess
{
    public interface IRecordUpdater
    {
        bool Update(
            EntityRecord entityRecord,
            object concurrencyCheckValue = null,
            Func<string> changeDecriber = null);
    }
}