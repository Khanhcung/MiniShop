namespace Product.API.Dtos
{
	public record CreateProductDto(string Sku, string Name, decimal Price);
	public record UpdateProductDto(string Name, decimal Price);
	public record ProductVm(string Sku, string Name, decimal Price, bool IsActive);
}
