using System;
using System.Collections.Generic;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public sealed class Notificator
    {
        public IDictionary<NotificateType, Queue<string>> Messages { get; private set; }

        public Notificator()
        {
            Messages = new Dictionary<NotificateType, Queue<string>>();
            foreach (NotificateType type in Enum.GetValues(typeof(NotificateType)))
            {
                Messages[type] = new Queue<string>();
            }
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

        private void Notificate(string message, NotificateType type)
        {
            Messages[type].Enqueue(message);
        }

        private void Notificate(string message, NotificateType type, params object[] args)
        {
            Messages[type].Enqueue(message.Fill(args));
        }
    }
}