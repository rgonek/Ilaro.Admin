using System;
namespace Ilaro.Admin.Core.Data
{
    public interface ICreatingRecords
    {
        string Create(Entity entity, Func<string> changeDescriber = null);
    }
}