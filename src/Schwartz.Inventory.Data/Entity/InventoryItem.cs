using System;
using System.Runtime.Serialization;

namespace Schwartz.Inventory.Data.Entity
{
	[DataContract]
	public class InventoryItem
	{
		public Guid Id{ get; internal set; }
		public DateTime Created { get; set; }
		[DataMember] public string Label { get; set; }
		[DataMember] public DateTime Expires { get; set; } 
	}
}