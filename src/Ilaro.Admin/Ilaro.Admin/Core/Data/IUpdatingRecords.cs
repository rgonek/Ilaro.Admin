using System;
namespace Ilaro.Admin.Core.Data
{
    public interface IUpdatingRecords
    {
        bool Update(EntityRecord entityRecord, Func<string> changeDecriber = null);
    }
}