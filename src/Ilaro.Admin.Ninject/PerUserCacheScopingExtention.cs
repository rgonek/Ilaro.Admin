using System.Web;
using Ninject.Activation;
using Ninject.Syntax;

namespace Ilaro.Admin.Ninject
{
    public static class PerUserCacheScopingExtention
    {
        private const string _prefix = "Notificator";

        private static string _cacheId
        {
            get
            {
                // In momnet of initialization we could not have a session object
                return _prefix + "_" + HttpContext.Current.Session.SessionID;
            }
        }

        public static void InPerUserCacheScope<T>(this IBindingInSyntax<T> parent)
        {
            parent.InScope(CacheScopeCallback);
        }

        private static object CacheScopeCallback(IContext context)
        {
            if (HttpRuntime.Cache.Get(_cacheId) == null)
            {
                HttpContext.Current.Cache.Insert(_cacheId, new object());
            }

            return HttpRuntime.Cache.Get(_cacheId);
        }
    }
}
