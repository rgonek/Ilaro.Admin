using System;
namespace Ilaro.Admin.Core.Data
{
    public interface ICreatingRecords
    {
        string Create(EntityRecord entityRecord, Func<string> changeDescriber = null);
    }
}