using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Commons.Notificator
{
	public class Notificator
	{
		private IDictionary<NotificateType, Queue<string>> messages;
		public IDictionary<NotificateType, Queue<string>> Messages
		{
			get { return messages; }
			private set { messages = value; }
		}

		public Notificator()
		{
			Messages = new Dictionary<NotificateType, Queue<string>>();
			foreach (NotificateType type in Enum.GetValues(typeof(NotificateType)))
			{
				Messages[type] = new Queue<string>();
			}
		}
	}
}