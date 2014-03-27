using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Commons
{
	public class AdminException : Exception
	{
		//public NotificateType NotificationType { get; set; }

		public AdminException()
			: base()
		{ }

		public AdminException(string message)
			: base(message)
		{
			//NotificationType = NotificateType.Error;
		}

		//public AdminException(string message, NotificateType notificationType)
		//	: base(message)
		//{
		//	NotificationType = notificationType;
		//}
	}
}