using System;
namespace Ilaro.Admin.Core.Data
{
    public interface IUpdatingRecords
    {
        bool Update(Entity entity, Func<string> changeDecriber = null);
    }
}