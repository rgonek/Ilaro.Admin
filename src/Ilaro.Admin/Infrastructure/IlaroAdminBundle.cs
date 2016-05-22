using System.Web.Optimization;

namespace Ilaro.Admin.Infrastructure
{
    public static class IlaroAdminBundle
    {
        public static Bundle New(string virtualPath)
        {
#if DEBUG
            // disable minification
            return new Bundle(virtualPath);
#else
            return new ScriptBundle(virtualPath);
#endif
        }
    }
}