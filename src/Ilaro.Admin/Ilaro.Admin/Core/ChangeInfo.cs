namespace Ilaro.Admin.Core
{
    public class ChangeInfo
    {
        public string EntityName { get; private set; }
        public EntityChangeType Type { get; private set; }
        public string Description { get; private set; }

        public ChangeInfo(string entityName, EntityChangeType type, string description = "")
        {
            EntityName = entityName;
            Type = type;
            Description = description;
        }
    }
}