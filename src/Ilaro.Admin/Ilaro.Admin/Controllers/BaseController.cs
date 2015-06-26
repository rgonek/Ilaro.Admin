using System.Web.Mvc;
using Ilaro.Admin.Commons.Notificator;

namespace Ilaro.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected BaseController(Notificator notificator)
        {
            Notificator = notificator;
        }

        #region Notificator

        protected readonly Notificator Notificator;

        protected void Success(string message)
        {
            Notificate(message, NotificateType.Success);
        }

        protected void Success(string message, params object[] args)
        {
            Notificate(message, NotificateType.Success, args);
        }

        protected void Info(string message)
        {
            Notificate(message, NotificateType.Info);
        }

        protected void Info(string message, params object[] args)
        {
            Notificate(message, NotificateType.Info, args);
        }

        protected void Warning(string message)
        {
            Notificate(message, NotificateType.Warning);
        }

        protected void Warning(string message, params object[] args)
        {
            Notificate(message, NotificateType.Warning, args);
        }

        protected void Error(string message)
        {
            Notificate(message, NotificateType.Danger);
        }

        protected void Error(string message, params object[] args)
        {
            Notificate(message, NotificateType.Danger, args);
        }

        private void Notificate(string message, NotificateType type)
        {
            Notificator.Messages[type].Enqueue(message);
        }

        private void Notificate(string message, NotificateType type, params object[] args)
        {
            Notificator.Messages[type].Enqueue(string.Format(message, args));
        }

        #endregion
    }
}