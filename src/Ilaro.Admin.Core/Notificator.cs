using System;
using System.Collections.Generic;
using Ilaro.Admin.Core.Extensions;

namespace Ilaro.Admin.Core
{
    public sealed class Notificator
    {
        public IDictionary<NotificateType, Queue<string>> Messages { get; private set; }
        private readonly IDictionary<string, string> _modelErrors;

        public Notificator()
        {
            Messages = new Dictionary<NotificateType, Queue<string>>();
            _modelErrors = new Dictionary<string, string>();
            foreach (NotificateType type in Enum.GetValues(typeof(NotificateType)))
            {
                Messages[type] = new Queue<string>();
            }
        }

        public void AddModelError(string key, string errorMessage)
        {
            _modelErrors[key] = errorMessage;
        }

        public IDictionary<string, string> GetModelErrors()
        {
            return _modelErrors;
        }

        public void ClearModelErrors()
        {
            _modelErrors.Clear();
        }

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
            Messages[type].Enqueue(message);
        }

        public void Notificate(string message, NotificateType type, params object[] args)
        {
            Messages[type].Enqueue(message.Fill(args));
        }
    }
}