using System;
namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordCreator
    {
        string Create(EntityRecord entityRecord, Func<string> changeDescriber = null);
    }
}