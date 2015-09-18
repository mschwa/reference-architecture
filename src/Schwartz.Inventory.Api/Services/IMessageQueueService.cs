using System;
using System.Collections.Generic;
using Schwartz.Inventory.Api.Infrastructure;
using Schwartz.Inventory.Data.Entity;

namespace Schwartz.Inventory.Api.Services
{
	public interface IMessageQueueService
	{
		void QueueInventoryItem(InventoryItem item);
		void BatchInventoryItems(List<InventoryItem> item);
		void CacheInventoryItem(IInventoryTakeMessageCache cache, InventoryItem item);
		void FlushLocalCache(IInventoryTakeMessageCache cache);
	}

	public class MessageQueueException : ApplicationException { }
}