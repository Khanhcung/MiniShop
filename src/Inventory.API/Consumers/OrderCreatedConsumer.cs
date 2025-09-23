
using BuildingBlocks.Messaging;
using Confluent.Kafka;
using Inventory.API.Repositories;
using Shared.Contracts;
using System.Net.WebSockets;
using System.Text.Json;

namespace Inventory.API.Consumers
{
	public class OrderCreatedConsumer : BackgroundService
	{
		private readonly IKafkaProducer _producer;
		private readonly IStockRepository _stockRepository;
		private readonly IConfiguration _cfg;

		public OrderCreatedConsumer(IKafkaProducer producer, IStockRepository repository, IConfiguration configuration)
		{							
			_producer = producer; _cfg = configuration; repository = _stockRepository;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var conf = new ConsumerConfig
			{
				BootstrapServers = _cfg["Kafka:BootstrapServers"],
				GroupId = "inventory-consumers",
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = false
			};
			using var c = new ConsumerBuilder<string, string>(conf).Build();
			c.Subscribe(_cfg["Kafka:Topics:OrderCreated"] ?? "order.created");


			while (!stoppingToken.IsCancellationRequested)
			{
				var cr = c.Consume(TimeSpan.FromMilliseconds(500));
				if (cr is null) continue;


				var evt = JsonSerializer.Deserialize<OrderCreated>(cr.Message.Value);
				if (evt is null) { c.Commit(cr); continue; }

				var ok = true; string? reason = null;
				foreach (var line in evt.Lines) {

					if (!await _stockRepository.CanReserveAsync(line.Sku, line.Qty))
					{ ok = false; reason = $"OUT_OF_STOCK: {line.Sku}"; break; }


				}


				if (ok)
				{
					foreach (var line in evt.Lines)
						await _stockRepository.ReserveAsync(line.Sku, line.Qty);

					var payload = JsonSerializer.Serialize(
						new InventoryReserved(evt.OrderId, evt.Lines.Select(l => new ReservedItem(l.Sku, l.Qty)).ToList(), DateTime.UtcNow));
					await _producer.ProduceAsync(_cfg["Kafka:Topics:InventoryReserved"] ?? "inventory.reserved", evt.OrderId, payload);
				}
				else
				{
					var payload = JsonSerializer.Serialize(new InventoryFailed(evt.OrderId, reason!, DateTime.UtcNow));
					await _producer.ProduceAsync(_cfg["Kafka:Topics:InventoryFailed"] ?? "inventory.failed", evt.OrderId, payload);
				}

				c.Commit(cr);
			
			}
		
		}
	}
}
