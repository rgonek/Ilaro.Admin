using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ilaro.Admin.Core.Configuration.Configurators;
using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Extensions;

namespace Ilaro.Admin.AspNetCore
{
    public class IlaroAdminMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IlaroAdminOptions _options;

        public IlaroAdminMiddleware(
            RequestDelegate next,
            IlaroAdminOptions options)
        {
            Guard.Argument(next, nameof(next)).NotNull();
            Guard.Argument(options, nameof(options)).NotNull();

            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            // Check authorization
            if (_options.AuthorizationPolicy.HasValue())
            {
                var authorizationService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
                var authzResult =
                    await authorizationService.AuthorizeAsync(httpContext.User, _options.AuthorizationPolicy);

                if (!authzResult.Succeeded)
                {
                    await httpContext.ForbidAsync();
                    return;
                }
            }

            var logger = httpContext.RequestServices.GetService<ILogger<IlaroAdminMiddleware>>();
            var configurators = httpContext.RequestServices.GetServices<IEntityConfigurator>();
            var entities = await Configure(configurators);
            var path = request.Path;

            await _next(httpContext);
        }

        private async Task<EntitiesCollection> Configure(IEnumerable<IEntityConfigurator> configurators)
        {
            var entities = await ConfigureEntities(configurators);
            await ConfigureProperties(configurators, entities);

            return entities;
        }

        private async Task<EntitiesCollection> ConfigureEntities(IEnumerable<IEntityConfigurator> configurators)
        {
            var entities = new EntitiesCollection();
            foreach (var configurator in configurators)
            {
                var entity = new Entity(configurator.CustomizersHolder.Type);
                entities.Add(entity);
                ((ConfiguratorsHolder)configurator.CustomizersHolder).CustomizeEntity(entity);
            }

            await Task.CompletedTask;

            return entities;
        }

        private async Task ConfigureProperties(IEnumerable<IEntityConfigurator> configurators, EntitiesCollection entities)
        {
            foreach (var configurator in configurators)
            {
                var entity = entities[configurator.CustomizersHolder.Type];
                ((ConfiguratorsHolder)configurator.CustomizersHolder).CustomizeProperties(entity, entities);
            }
            await Task.CompletedTask;
        }
    }
}
