using System;

namespace Ilaro.Admin.Core
{
    public class Configuration : IConfiguration
    {
        private readonly IConfigurationProvider _configurationProvider;

        public Configuration(IConfigurationProvider configurationProvider)
        {
            if (configurationProvider == null)
                throw new ArgumentNullException(nameof(configurationProvider));

            _configurationProvider = configurationProvider;
        }

        public int ItemsQuantityPerPage
        {
            get { return _configurationProvider.Get("Ilaro.Admin.ItemsQuantityPerPage", 10); }
        }

        public string PageRequestName
        {
            get { return _configurationProvider.Get("Ilaro.Admin.PageRequestName", "page"); }
        }

        public string SearchQueryRequestName
        {
            get { return _configurationProvider.Get("Ilaro.Admin.SearchQueryRequestName", "sq"); }
        }

        public string PerPageRequestName
        {
            get { return _configurationProvider.Get("Ilaro.Admin.PerPageRequestName", "pp"); }
        }

        public string OrderRequestName
        {
            get { return _configurationProvider.Get("Ilaro.Admin.OrderRequestName", "o"); }
        }

        public string OrderDirectionRequestName
        {
            get { return _configurationProvider.Get("Ilaro.Admin.OrderDirectionRequestName", "od"); }
        }

        public long MaxFileSize
        {
            get { return _configurationProvider.Get("Ilaro.Admin.MaxFileSize", 2048000); }
        }

        public string[] AllowedFileExtensions
        {
            get
            {
                return _configurationProvider
                    .Get("Ilaro.Admin.AllowedFileExtensions", ".jpg,.jpeg,.png,.gif,.bmp")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string UploadFilesTempFolderSufix
        {
            get { return _configurationProvider.Get("Ilaro.Admin.UploadFilesTempFolderSufix", "_temp"); }
        }
    }
}