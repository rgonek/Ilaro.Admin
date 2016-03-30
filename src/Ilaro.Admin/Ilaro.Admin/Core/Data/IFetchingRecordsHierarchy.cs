namespace Ilaro.Admin.Core.Data
{
    public interface IFetchingRecordsHierarchy
    {
        RecordHierarchy GetRecordHierarchy(EntityRecord entityRecord);
    }
}