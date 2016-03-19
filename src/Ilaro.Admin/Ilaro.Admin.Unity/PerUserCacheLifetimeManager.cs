using System.Web;
using Microsoft.Practices.Unity;

namespace Ilaro.Admin.Commons
{
    public class PerUserCacheLifetimeManager : LifetimeManager
    {
        private const string _prefix = "Notificator:";

        private string _cacheId
        {
            get
            {
                // In momnet of initialization we could not have a session object
                return _prefix + "_" + HttpContext.Current.Session.SessionID;
            }
        }

        public override object GetValue()
        {
            return HttpRuntime.Cache.Get(_cacheId);
        }

        public override void RemoveValue()
        {
            HttpRuntime.Cache.Remove(_cacheId);
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Cache.Insert(_cacheId, newValue);
        }
    }
}