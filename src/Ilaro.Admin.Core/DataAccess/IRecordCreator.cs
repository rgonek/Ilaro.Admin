using System;
namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordCreator
    {
        IdValue Create(EntityRecord entityRecord);
    }
}