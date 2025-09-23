using BuildingBlocks.Data;
using BuildingBlocks.Extensions;
using Dapper;
using Order.API.Dtos;
using System.Data.Common;

namespace Order.API.Repositories
{
	public class OrderRepository : IOrderRepository
	{

		private readonly SqlConnectionFactory _connectionFactory;


		public OrderRepository( SqlConnectionFactory factory)
		{
				_connectionFactory = factory;
		}

		public async Task<Guid> CreateAsync(string code, IEnumerable<(string sku, int qty, decimal unitPrice)> lines, decimal total)
		{
			using var db = await _connectionFactory.Create().EnsureOpenAsync();
			using var transaction = db.BeginTransaction();

			var id = Guid.NewGuid();
			await db.ExecuteAsync("""
            INSERT INTO Orders(Id,Code,Status,TotalAmount) VALUES(@Id,@Code,'Pending',@Total)
        """, new { Id = id, Code = code, Total = total }, transaction);
			foreach (var l in lines)
			{
				await db.ExecuteAsync("""
                INSERT INTO OrderLines(OrderId,Sku,Qty,UnitPrice) VALUES(@Id,@Sku,@Qty,@Price)
            """, new { Id = id, Sku = l.sku, Qty = l.qty, Price = l.unitPrice }, transaction);
			}
			transaction.Commit();
			return id;



		}

		public async Task<OrderVm?> GetAsync(Guid id)
		{
			using var db = _connectionFactory.Create();
			return await db.QuerySingleOrDefaultAsync<OrderVm>(
				"SELECT Id,Code,Status,TotalAmount,CreatedAt FROM Orders WHERE Id=@id", new { id });
		}

		public async Task UpdateStatusAsync(Guid id, string status)
		{
			using var db = _connectionFactory.Create();
			await db.ExecuteAsync("UPDATE Orders SET Status=@Status WHERE Id=@Id", new { Id = id, Status = status });
		}
	}
}
