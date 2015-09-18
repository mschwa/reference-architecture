using System.Collections.Generic;
using Schwartz.Inventory.Data.Entity;

namespace Schwartz.Inventory.Api.Infrastructure
{
	public interface IInventoryTakeMessageCache
	{
		List<InventoryItem> Items { get; set; }
	}

	public class InventoryTakeMessageCache : IInventoryTakeMessageCache
	{
		public List<InventoryItem> Items { get; set; }
	}
}