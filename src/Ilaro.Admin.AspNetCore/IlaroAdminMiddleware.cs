using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

            if (_options.AuthorizationPolicy.HasValue())
            {
                var authorizationService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
                var authzResult = await authorizationService.AuthorizeAsync(httpContext.User, _options.AuthorizationPolicy);

                if (!authzResult.Succeeded)
                {
                    await httpContext.ForbidAsync();
                    return;
                }
            }

            //var logger = httpContext.RequestServices.GetService<ILogger<IlaroAdminMiddleware>>();
            //var entities = httpContext.RequestServices.GetService<IEntityCollection>();
            //var path = request.Path;

            await _next(httpContext);
        }
    }
}
