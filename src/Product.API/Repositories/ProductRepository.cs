using BuildingBlocks.Data;
using Dapper;
using Product.API.Dtos;

namespace Product.API.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly SqlConnectionFactory _connectionFactory;

		public ProductRepository(SqlConnectionFactory f)
		{
			_connectionFactory = f;
		}
		public async Task CreateAsync(CreateProductDto dto)
		{
			using var db = _connectionFactory.Create();
			await db.ExecuteAsync("""
            INSERT INTO Products(Sku,Name,Price,IsActive) VALUES(@Sku,@Name,@Price,1)
        """, dto);
		}

		public async Task DeleteAsync(string sku)
		{
			using var db = _connectionFactory.Create();
			await db.ExecuteAsync("UPDATE Products SET IsActive=0 WHERE Sku=@sku", new { sku });
		}

		public async Task<ProductVm?> GetAsync(string sku)
		{
			using var db = _connectionFactory.Create();
			return await db.QuerySingleOrDefaultAsync<ProductVm>(@"
            SELECT Sku,Name,Price,IsActive FROM Products WHERE Sku=@sku", new { sku });
		}

		public async Task UpdateAsync(string sku, UpdateProductDto dto)
		{
			using var db = _connectionFactory.Create();
			await db.ExecuteAsync("""
            UPDATE Products SET Name=@Name, Price=@Price WHERE Sku=@Sku
        """, new { dto.Name, dto.Price, Sku = sku });
		}
	}
}
