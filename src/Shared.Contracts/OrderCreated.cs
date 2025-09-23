using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
	public record OrderCreated(string OrderId, string Code, List<OrderLine> Lines, DateTime OccurredAt);
	public record OrderLine(string Sku, int Qty, decimal UnitPrice);
}
