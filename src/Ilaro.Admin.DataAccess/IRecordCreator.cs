using System;
namespace Ilaro.Admin.DataAccess
{
    public interface IRecordCreator
    {
        string Create(EntityRecord entityRecord, Func<string> changeDescriber = null);
    }
}