using Inventory.API.Dtos;

namespace Inventory.API.Repositories
{
	public interface IStockRepository
	{
		Task UpsertAdjustAsync(string sku, int deltaOnHand);
		Task<StockVm?> GetAsync(string sku);
		Task<bool> CanReserveAsync(string sku, int needQty);
		Task ReserveAsync(string sku, int qty);
	}
}
