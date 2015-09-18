using System;

namespace Schwartz.Inventory.Notify
{
	public interface INotificationService
	{
		void SendText(string number, string text);
		void SendEmail(string to, string from, string subject, string body);
	}

	public class NotificationServiceUnavailable : Exception
	{
		public NotificationServiceUnavailable(Exception inner) :
			base("Notification Service is failing. Ensure that the Network Credentials are correct. Etc...", inner)
		{
		}
	}
}