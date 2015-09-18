using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceBus.Messaging;
using Schwartz.Inventory.Api.Infrastructure;
using Schwartz.Inventory.Data.Entity;

namespace Schwartz.Inventory.Api.Services
{
	public class MessageQueueService : IMessageQueueService
	{
		private readonly ApplicationSettings _settings;

		public MessageQueueService(ApplicationSettings settings)
		{
			_settings = settings;
		}

		public void QueueInventoryItem(InventoryItem item)
		{
			var client = QueueClient.CreateFromConnectionString(_settings.ServiceBusConnection, _settings.QueueName);
			var message = new BrokeredMessage {Label = item.Label};
			client.Send(message);

			client.Close();
		}

		public void BatchInventoryItems(List<InventoryItem> items)
		{
			if (items == null || items.Count <= 0) return;

			var client = QueueClient.CreateFromConnectionString(_settings.ServiceBusConnection, _settings.QueueName);
			client.SendBatch(items.Select(item => new BrokeredMessage {Label = item.Label}));

			client.Close();
		}

		public void CacheInventoryItem(IInventoryTakeMessageCache cache, InventoryItem item)
		{
			cache.Items.Add(item);
		}

		public void FlushLocalCache(IInventoryTakeMessageCache cache)
		{
			BatchInventoryItems(cache.Items);
		}
	}
}