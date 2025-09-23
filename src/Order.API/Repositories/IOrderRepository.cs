using Order.API.Dtos;

namespace Order.API.Repositories
{
	public interface IOrderRepository
	{
		Task<Guid> CreateAsync(string code, IEnumerable<(string sku, int qty, decimal unitPrice)> lines, decimal total);
		Task<OrderVm?> GetAsync(Guid id);
		Task UpdateStatusAsync(Guid id, string status);
	}
}
