using Dawn;
using Ilaro.Admin.AspNetCore;
using System;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        private static readonly string _defaultPath = "/ilaroadmin";

        public static IApplicationBuilder UseIlaroAdmin(this IApplicationBuilder builder)
            => builder.UseIlaroAdmin(null, new IlaroAdminOptions());

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            string path)
            => builder.UseIlaroAdmin(path, new IlaroAdminOptions());

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            string path,
            Action<IlaroAdminOptions> configure)
        {
            var options = new IlaroAdminOptions();
            configure(new IlaroAdminOptions());

            return builder.UseIlaroAdmin(path, options);
        }

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            Action<IlaroAdminOptions> configure)
        {
            var options = new IlaroAdminOptions();
            configure(options);

            return builder.UseIlaroAdmin(_defaultPath, options);
        }

        public static IApplicationBuilder UseIlaroAdmin(
            this IApplicationBuilder builder,
            IlaroAdminOptions options)
            => builder.UseIlaroAdmin(_defaultPath, options);

        public static IApplicationBuilder UseIlaroAdmin(this IApplicationBuilder app, string path, IlaroAdminOptions options)
        {
            Guard.Argument(app, nameof(app)).NotNull();
            Guard.Argument(options, nameof(options)).NotNull();
            if (path == null)
                path = _defaultPath;

            //app.ApplicationServices.GetService()

            return app.MapWhen(x => x.Request.Path.StartsWithSegments(path), b => b.UseMiddleware<IlaroAdminMiddleware>(options));
        }
    }
}
