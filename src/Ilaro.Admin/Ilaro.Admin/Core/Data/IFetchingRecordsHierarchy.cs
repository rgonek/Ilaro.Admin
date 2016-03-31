namespace Ilaro.Admin.Core.Data
{
    public interface IFetchingRecordsHierarchy
    {
        RecordHierarchy GetRecordHierarchy(EntityRecord entityRecord);
        EntityHierarchy GetEntityHierarchy(Entity entity);
    }
}