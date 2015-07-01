using System;

namespace Ilaro.Admin.Core
{
    public class Configuration : IConfiguration
    {
        private readonly IConfigurationProvider _configurationProvider;

        public Configuration(IConfigurationProvider configurationProvider)
        {
            if (configurationProvider == null)
                throw new ArgumentNullException("configurationProvider");

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
    }
}