using Dawn;
using Microsoft.Extensions.Configuration;
using System;

namespace Ilaro.Admin.Core
{
    public sealed class AppConfiguration : IAppConfiguration
    {
        private readonly IlaroAdminOptions _options;

        public AppConfiguration(IConfiguration configuration, IIlaroAdminOptions options)
        {
            Guard.Argument(configuration, nameof(configuration)).NotNull();
            Guard.Argument(options, nameof(options)).NotNull();
        }

        public int ItemsQuantityPerPage => _options.ItemsQuantityPerPage ?? 10;

        public string PageRequestName => _options.PageRequestName ?? "page";

        public string SearchQueryRequestName => _options.SearchQueryRequestName ?? "sq";

        public string PerPageRequestName => _options.PerPageRequestName ?? "pp";

        public string OrderRequestName => _options.OrderRequestName ?? "o";

        public string OrderDirectionRequestName => _options.OrderDirectionRequestName ?? "od";

        public long MaxFileSize => _options.MaxFileSize ?? 2048000;

        public string[] AllowedFileExtensions => (_options.AllowedFileExtensions ?? ".jpg,.jpeg,.png,.gif,.bmp")
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        public string UploadFilesTempFolderSufix => _options.UploadFilesTempFolderSufix ?? "_temp";
    }
}