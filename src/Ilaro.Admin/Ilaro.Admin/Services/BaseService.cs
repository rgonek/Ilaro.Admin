using Ilaro.Admin.Commons.Notificator;

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
            _notificator.Messages[type].Enqueue(message);
		}

	    private void Notificate(
            string message, 
            NotificateType type, 
            params object[] args)
		{
            _notificator.Messages[type].Enqueue(string.Format(message, args));
		}

		#endregion
	}
}