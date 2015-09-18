using System;
using System.ComponentModel;
using Schwartz.Inventory.Data.Entity;

namespace Schwartz.Inventory.Data.Repository
{
	public interface IInventoryRepository
	{
		InventoryItem Create(InventoryItem item);

		InventoryItem Get(string label);

		InventoryItem Remove(InventoryItem item);
	}

	public class InventoryFatalException : Exception
	{
		public InventoryFatalException(Exception inner) :
			base("Database is failing. Ensure that the connection string is correct. Etc...", inner)
		{
		}
	}
}