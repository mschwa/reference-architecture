using System;
using System.Collections.Generic;
using Schwartz.Inventory.Data.Entity;

namespace Schwartz.Inventory.Data.Repository
{
	public class InventoryRepository : IInventoryRepository
	{
		private readonly Dictionary<string, InventoryItem> _source; 
			 
		public InventoryRepository()
		{
			_source = new Dictionary<string, InventoryItem>();
		}

		public InventoryItem Create(InventoryItem item)
		{
			try
			{
				item.Id = Guid.NewGuid();
				item.Created = DateTime.UtcNow;

				if (!_source.ContainsKey(item.Label.ToLower()))
				{
					_source.Add(item.Label.ToLower(), item);
					return item;
				}

				// Duplicate. Using null here as throwing an exception costs overhead.
				return null;
			}
			catch (Exception ex)
			{
				throw new InventoryFatalException(ex);
			}
		}

		public InventoryItem Get(string label)
		{
			try
			{
				return _source.ContainsKey(label.ToLower()) ? _source[label.ToLower()] : null;
			}
			catch (Exception ex)
			{
				throw new InventoryFatalException(ex);
			}
			
		}

		public InventoryItem Remove(InventoryItem item)
		{
			try
			{
				if (_source.ContainsKey(item.Label.ToLower()))
				{
					_source.Remove(item.Label);
					return item;
				}

				return null;
			}
			catch (Exception ex)
			{
				throw new InventoryFatalException(ex);
			}
			
		}
	}
}