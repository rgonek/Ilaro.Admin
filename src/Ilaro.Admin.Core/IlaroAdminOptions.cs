namespace Ilaro.Admin.Core
{
    public class IlaroAdminOptions
    {
        public int? ItemsQuantityPerPage { get; }

        public string PageRequestName { get; }

        public string SearchQueryRequestName { get; }

        public string PerPageRequestName { get; }

        public string OrderRequestName { get; }

        public string OrderDirectionRequestName { get; }

        public long? MaxFileSize { get; }

        public string AllowedFileExtensions { get; }

        public string UploadFilesTempFolderSufix { get; }
    }
}
