using System.Configuration;
using Microsoft.WindowsAzure;

namespace Schwartz.Inventory.Api.Infrastructure
{
	public class ApplicationSettings
	{
		public ApplicationSettings(bool loadOnConstruction = true)
		{
			if (loadOnConstruction)
			{
				EnableMessageCache = bool.Parse(ConfigurationManager.AppSettings["system.enableMessageCache"]);
				AdminEmail = ConfigurationManager.AppSettings["system.admin.email"];
				NoReplyEmail = ConfigurationManager.AppSettings["system.noreply.email"];
				AudienceSecret = ConfigurationManager.AppSettings["as.AudienceSecret"];
				AudienceId = ConfigurationManager.AppSettings["as.AudienceId"];
				ServiceBusConnection = CloudConfigurationManager.GetSetting("system.bus.connection");
				QueueName = ConfigurationManager.AppSettings["system.queue.name"];
				BaseUrl = ConfigurationManager.AppSettings["system.api.baseurl"];
			}
		}

		public bool EnableMessageCache { get; set; }
		public string AdminEmail { get; set; }
		public string NoReplyEmail { get; set; }
		public string AudienceId { get; set; }
		public string AudienceSecret { get; set; }
		public string ServiceBusConnection { get; set; }
		public string QueueName { get; set; }
		public string BaseUrl { get; set; }
	}
}