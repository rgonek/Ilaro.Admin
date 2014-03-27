using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Commons
{
	public class UnityPerUserCacheLifetimeManager : LifetimeManager
	{
		private string prefix;

		private string cacheId
		{
			get
			{
				// In momnet of initialization we could not have a session object
				return prefix + "_" + HttpContext.Current.Session.SessionID;
			}
		}

		public UnityPerUserCacheLifetimeManager(string prefix)
		{
			this.prefix = prefix;
		}

		public override object GetValue()
		{
			return HttpRuntime.Cache.Get(cacheId);
		}

		public override void RemoveValue()
		{
			HttpRuntime.Cache.Remove(cacheId);
		}

		public override void SetValue(object newValue)
		{
			HttpContext.Current.Cache.Insert(cacheId, newValue);
		}
	}
}