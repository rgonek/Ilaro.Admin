using Ilaro.Admin.Core;

namespace Ilaro.Admin.Services
{
    public class BaseService
    {
        protected BaseService(Notificator notificator)
        {
            _notificator = notificator;
        }

        #region Notificator

        private readonly Notificator _notificator;

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
            _notificator.Messages[type].Enqueue(message);
        }

        protected virtual void Notificate(
            string message,
            NotificateType type,
            params object[] args)
        {
            _notificator.Messages[type].Enqueue(string.Format(message, args));
        }

        #endregion
    }
}