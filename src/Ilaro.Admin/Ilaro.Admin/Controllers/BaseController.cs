using Ilaro.Admin.Commons.Notificator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ilaro.Admin.Controllers
{
	public partial class BaseController : Controller
	{
		public BaseController(Notificator notificator)
		{
			this.notificator = notificator;
		}

		[NonAction]
		public virtual ActionResult SafeRedirect(string returnUrl)
		{
			if (!String.IsNullOrWhiteSpace(returnUrl)
				&& Url.IsLocalUrl(returnUrl)
				&& returnUrl.Length > 1
				&& returnUrl.StartsWith("/", StringComparison.Ordinal)
				&& !returnUrl.StartsWith("//", StringComparison.Ordinal)
				&& !returnUrl.StartsWith("/\\", StringComparison.Ordinal))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		#region Notificator

		protected readonly Notificator notificator;

		public void Success(string message)
		{
			Notificate(message, NotificateType.Success);
		}

		public void Success(string message, params object[] args)
		{
			Notificate(message, NotificateType.Success, args);
		}

		public void Info(string message)
		{
			Notificate(message, NotificateType.Info);
		}

		public void Info(string message, params object[] args)
		{
			Notificate(message, NotificateType.Info, args);
		}

		public void Warning(string message)
		{
			Notificate(message, NotificateType.Warning);
		}

		public void Warning(string message, params object[] args)
		{
			Notificate(message, NotificateType.Warning, args);
		}

		public void Error(string message)
		{
			Notificate(message, NotificateType.Danger);
		}

		public void Error(string message, params object[] args)
		{
			Notificate(message, NotificateType.Danger, args);
		}

		public void Notificate(string message, NotificateType type)
		{
			notificator.Messages[type].Enqueue(message);
		}

		public void Notificate(string message, NotificateType type, params object[] args)
		{
			notificator.Messages[type].Enqueue(string.Format(message, args));
		}

		#endregion
	}
}