﻿namespace Ilaro.Admin.Core
{
    public interface IIlaroAdminOptions
    {
        string ConnectionStringName { get; set; }

        string RoutePrefix { get; set; }

        int? ItemsQuantityPerPage { get; }

        string PageRequestName { get; }

        string SearchQueryRequestName { get; }

        string PerPageRequestName { get; }

        string OrderRequestName { get; }

        string OrderDirectionRequestName { get; }

        long? MaxFileSize { get; }

        string AllowedFileExtensions { get; }

        string UploadFilesTempFolderSufix { get; }
    }
}
