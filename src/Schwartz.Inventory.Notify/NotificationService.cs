using System;
using System.Net;
using System.Net.Mail;
using SendGrid;

namespace Schwartz.Inventory.Notify
{
	public class NotificationService : INotificationService
	{
		public void SendText(string number, string text)
		{
			throw new NotImplementedException();
		}

		public async void SendEmail(string to, string @from, string subject, string body)
		{
			// Create the email object first, then add the properties.
			var myMessage = new SendGridMessage();
			myMessage.AddTo(to);
			myMessage.From = new MailAddress(@from);
			myMessage.Subject = subject;
			myMessage.Text = body;

			try
			{
				// Create credentials, specifying your user name and password.
				var credentials = new NetworkCredential("azure_a8dd6f8629cccb8ea2e88e269697cbf0@azure.com", "YWKFjt2WbmKtpB4");

				// Create an Web transport for sending email.
				var transportWeb = new Web(credentials);
				await transportWeb.DeliverAsync(myMessage);
			}
			catch (Exception ex)
			{
				throw new NotificationServiceUnavailable(ex);
			}
		}
	}
}