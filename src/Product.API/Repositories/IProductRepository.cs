using Product.API.Dtos;

namespace Product.API.Repositories
{
	public interface IProductRepository
	{
		Task CreateAsync(CreateProductDto dto);
		Task<ProductVm?> GetAsync(string sku);
		Task UpdateAsync(string sku, UpdateProductDto dto);
		Task DeleteAsync(string sku); // soft delete
	}
}
