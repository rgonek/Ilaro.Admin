namespace Ilaro.Admin.ViewComponents
{
    public sealed class BreadcrumbItem
    {
        public string DisplayName { get; }

        public string Page { get; }

        public string EntityName { get; }

        public bool IsActive { get; }

        public BreadcrumbItem(string displayName, string page, string currentActivePage)
            : this(displayName, page, null, currentActivePage)
        {
        }

        public BreadcrumbItem(string displayName, string page, bool isActive)
            : this(displayName, page, null, isActive)
        {
        }

        public BreadcrumbItem(string displayName, string page, string entityName, string currentActivePage)
            : this(displayName, page, entityName, page == currentActivePage)
        {
        }

        public BreadcrumbItem(string displayName, string page, string entityName, bool isActive)
        {
            DisplayName = displayName;
            Page = page;
            EntityName = entityName;
            IsActive = isActive;
        }
    }
}
