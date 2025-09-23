using BuildingBlocks.Data;
using Dapper;
using Inventory.API.Dtos;

namespace Inventory.API.Repositories
{
	public class StockRepository : IStockRepository
	{
		private readonly SqlConnectionFactory _connectionFactory;

		public StockRepository(SqlConnectionFactory factory)
		{
				_connectionFactory = factory;
		}

		public async Task<bool> CanReserveAsync(string sku, int needQty)
		{
			using var db = _connectionFactory.Create();
			var row = await db.QuerySingleOrDefaultAsync<(int OnHand, int Reserved)>(
				"SELECT OnHand, Reserved FROM Stocks WHERE Sku=@sku", new { sku });
			if (row == default) return false;  
        return (row.OnHand - row.Reserved) >= needQty;
		}

		public async Task<StockVm?> GetAsync(string sku)
		{
			using var db = _connectionFactory.Create();
			return await db.QuerySingleOrDefaultAsync<StockVm>(
				"SELECT Sku, OnHand, Reserved FROM Stocks WHERE Sku=@sku", new { sku });
		}

		public async Task ReserveAsync(string sku, int qty)
		{
			using var db = _connectionFactory.Create();
			await db.ExecuteAsync("UPDATE Stocks SET Reserved = Reserved + @Qty, UpdatedAt = SYSDATETIME() WHERE Sku=@sku",
				new { sku, Qty = qty });
		}

		public async Task UpsertAdjustAsync(string sku, int deltaOnHand)
		{
			using var db = _connectionFactory.Create();
			await db.ExecuteAsync("""
        MERGE Stocks AS t
        USING (SELECT @Sku AS Sku) AS s ON t.Sku = s.Sku
        WHEN MATCHED THEN UPDATE SET OnHand = t.OnHand + @Delta, UpdatedAt = SYSDATETIME()
        WHEN NOT MATCHED THEN INSERT (Sku, OnHand, Reserved) VALUES(@Sku, @InitOnHand, 0);
        """, new { Sku = sku, Delta = deltaOnHand, InitOnHand = deltaOnHand });
		}
	}
}
