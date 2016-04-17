using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using RazorGenerator.Mvc;

namespace Ilaro.Admin
{
    /// <summary>
    /// Code from http://razorgenerator.codeplex.com/workitem/61
    /// </summary>
    public class IlaroPrecompiledMvcEngine : PrecompiledMvcEngine
    {
        private readonly string _baseVirtualPath;

        public IlaroPrecompiledMvcEngine(Assembly assembly)
            : base(assembly, null)
        {
        }

        public IlaroPrecompiledMvcEngine(Assembly assembly, string baseVirtualPath)
            : base(assembly, baseVirtualPath)
        {

            if (!String.IsNullOrEmpty(baseVirtualPath))
            {
                // For a virtual path to combine properly, it needs to start with a ~/ and end with a /.
                if (!baseVirtualPath.StartsWith("~/", StringComparison.Ordinal))
                {
                    baseVirtualPath = "~/" + baseVirtualPath;
                }
                if (!baseVirtualPath.EndsWith("/", StringComparison.Ordinal))
                {
                    baseVirtualPath += "/";
                }
            }
            _baseVirtualPath = baseVirtualPath;
        }

        public bool AlwaysUsePhysicalViews { get; set; }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            if (AlwaysUsePhysicalViews && PhysicalFileExists(virtualPath))
            {
                return false;
            }
            return base.FileExists(controllerContext, virtualPath);
        }

        private bool PhysicalFileExists(string virtualPath)
        {
            if (!virtualPath.StartsWith(_baseVirtualPath ?? String.Empty, StringComparison.Ordinal))
                return false;

            // If a base virtual path is specified, we should remove it as a prefix. Everything that follows should map to a view file on disk.
            if (!String.IsNullOrEmpty(_baseVirtualPath))
            {
                virtualPath = '~' + virtualPath.Substring(_baseVirtualPath.Length);
            }

            var path = HttpContext.Current.Request.MapPath(virtualPath);
            return File.Exists(path);
        }

        public new object CreateInstance(string virtualPath)
        {
            if (AlwaysUsePhysicalViews && PhysicalFileExists(virtualPath))
            {
                // If the physical file on disk is exists and the user's opted in this behavior, serve it instead.
                return BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(WebViewPage));
            }

            return base.CreateInstance(virtualPath);
        }
    }
}
