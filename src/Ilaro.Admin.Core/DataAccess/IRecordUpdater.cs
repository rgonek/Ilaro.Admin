namespace Ilaro.Admin.Core.DataAccess
{
    public interface IRecordUpdater
    {
        bool Update(
            EntityRecord entityRecord,
            object concurrencyCheckValue = null);
    }
}