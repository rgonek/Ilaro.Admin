using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core.File;
using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Extensions;

namespace Ilaro.Admin.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string GetImageMinPath(
            this UrlHelper urlHelper,
            PropertyValue value)
        {
            if (value.AsString.IsNullOrEmpty())
            {
                return null;
            }

            var settings = value.Property.FileOptions.Settings.LastOrDefault();
            var path = Pather.Combine("~/", value.Property.FileOptions.Path, settings.SubPath, value.AsString).Replace("\\", "/");

            return urlHelper.Content(path);
        }

        public static string GetImageBigPath(
            this UrlHelper urlHelper,
            PropertyValue value)
        {
            if (value.AsString.IsNullOrEmpty())
            {
                return null;
            }

            var settings = value.Property.FileOptions.Settings.FirstOrDefault();
            var path = Pather.Combine("~/", value.Property.FileOptions.Path, settings.SubPath, value.AsString).Replace("\\", "/");

            return urlHelper.Content(path);
        }

        public static string GetFilePath(
            this UrlHelper urlHelper,
            PropertyValue value)
        {
            if (value.AsString.IsNullOrEmpty())
            {
                return null;
            }

            var path = Pather.Combine("~/", value.Property.FileOptions.Path, value.AsString).Replace("\\", "/");

            return urlHelper.Content(path);
        }
    }
}