using Autofac;
using log4net;
using Microsoft.Owin.Security;
using Schwartz.Inventory.Api.Providers;
using Schwartz.Inventory.Api.Services;
using Schwartz.Inventory.Data.Repository;
using Schwartz.Inventory.Notify;

namespace Schwartz.Inventory.Api.Infrastructure
{
	public class DependencyContainer : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<CustomJwtFormat>()
				.As<ISecureDataFormat<AuthenticationTicket>>();

			builder.RegisterType<MessageQueueService>()
				.As<IMessageQueueService>()
				.InstancePerRequest();

			builder.RegisterType<NotificationService>()
				.As<INotificationService>()
				.InstancePerRequest();

			//  Mock Database
			builder.RegisterType<InventoryRepository>()
				.As<IInventoryRepository>()
				.SingleInstance();

			builder.RegisterType<InventoryTakeMessageCache>()
				.As<IInventoryTakeMessageCache>()
				.SingleInstance();

			builder.RegisterInstance(LogManager.GetLogger("Inventory"))
				.As<ILog>()
				.SingleInstance();

			base.Load(builder);
		}
	}
}