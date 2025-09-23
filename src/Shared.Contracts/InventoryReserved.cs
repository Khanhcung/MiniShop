using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
	public record InventoryReserved(string OrderId, List<ReservedItem> Items, DateTime OccurredAt);
	public record ReservedItem(string Sku, int Qty);
}
