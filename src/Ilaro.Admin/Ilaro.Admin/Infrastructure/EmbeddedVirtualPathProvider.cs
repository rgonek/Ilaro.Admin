using System;
using System.Collections;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;

namespace Ilaro.Admin.Infrastructure
{
    public class EmbeddedVirtualPathProvider : VirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            if (IsEmbeddedPath(virtualPath))
                return true;

            return base.FileExists(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsEmbeddedPath(virtualPath))
                return null;

            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsEmbeddedPath(virtualPath))
            {
                var fileNameWithExtension = virtualPath.Substring(virtualPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
                var @namespace = typeof(EmbeddedVirtualPathProvider)
                                .Assembly
                                .GetName()
                                .Name;
                var folder = GetFolderName(fileNameWithExtension);
                var manifestResourceName = string.Format("{0}.{1}.{2}", @namespace, folder, fileNameWithExtension);
                var stream = typeof(EmbeddedVirtualPathProvider).Assembly.GetManifestResourceStream(manifestResourceName);
                return new EmbeddedVirtualFile(virtualPath, stream);
            }
            return base.GetFile(virtualPath);
        }

        private static string GetFolderName(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            switch (extension.ToLower())
            {
                case ".js":
                    return "Scripts";
                case ".css":
                    return "Content.css";
                case ".gif":
                case ".png":
                case ".jpg":
                    return "Content.img";
                default:
                    return "Content.fonts";
            }
        }

        private static bool IsEmbeddedPath(string path)
        {
            //var prefix = string.IsNullOrWhiteSpace(Admin.RoutesPrefix) ?
                //"IlaroAdmin" :
                //Admin.RoutesPrefix;
            return path.Contains("~/ira/");
        }
    }
}