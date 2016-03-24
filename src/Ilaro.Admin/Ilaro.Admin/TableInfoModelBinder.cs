using System;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Models;

namespace Ilaro.Admin
{
    public class TableInfoModelBinder : DefaultModelBinder
    {
        private readonly IConfiguration _configuration;

        public TableInfoModelBinder(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.RequestContext.HttpContext.Request.Params;
            var model = new TableInfo
            {
                Page = TryConvert(request[_configuration.PageRequestName], 1),
                PerPage = TryConvert(request[_configuration.PerPageRequestName], _configuration.ItemsQuantityPerPage),
                SearchQuery = TryConvert(request[_configuration.SearchQueryRequestName], ""),
                Order = TryConvert(request[_configuration.OrderRequestName], ""),
                OrderDirection = TryConvert(request[_configuration.OrderDirectionRequestName], "")
            };

            return model;
        }

        private static TOutput TryConvert<TOutput>(string value, TOutput defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return (TOutput)Convert.ChangeType(value, typeof(TOutput));
        }
    }
}