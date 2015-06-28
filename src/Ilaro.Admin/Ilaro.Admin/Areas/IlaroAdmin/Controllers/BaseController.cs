using System.Web.Mvc;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Areas.IlaroAdmin.Controllers
{
    public class BaseController : Controller
    {
        protected BaseController(Notificator notificator)
        {
            Notificator = notificator;
        }

        #region Notificator

        protected readonly Notificator Notificator;

        protected virtual void Success(string message)
        {
            Notificate(message, NotificateType.Success);
        }

        protected virtual void Success(string message, params object[] args)
        {
            Notificate(message, NotificateType.Success, args);
        }

        protected virtual void Info(string message)
        {
            Notificate(message, NotificateType.Info);
        }

        protected virtual void Info(string message, params object[] args)
        {
            Notificate(message, NotificateType.Info, args);
        }

        protected virtual void Warning(string message)
        {
            Notificate(message, NotificateType.Warning);
        }

        protected virtual void Warning(string message, params object[] args)
        {
            Notificate(message, NotificateType.Warning, args);
        }

        protected virtual void Error(string message)
        {
            Notificate(message, NotificateType.Danger);
        }

        protected virtual void Error(string message, params object[] args)
        {
            Notificate(message, NotificateType.Danger, args);
        }

        protected virtual void Notificate(string message, NotificateType type)
        {
            Notificator.Messages[type].Enqueue(message);
        }

        protected virtual void Notificate(string message, NotificateType type, params object[] args)
        {
            Notificator.Messages[type].Enqueue(string.Format(message, args));
        }

        #endregion
    }
}
