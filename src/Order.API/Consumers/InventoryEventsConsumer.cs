
using Confluent.Kafka;
using Order.API.Repositories;
using Shared.Contracts;
using System.Text.Json;

namespace Order.API.Consumers
{
	public class InventoryEventsConsumer : BackgroundService
	{
		private readonly IConfiguration _cfg;
		private readonly IOrderRepository _repo;

		public InventoryEventsConsumer(IConfiguration configuration, IOrderRepository orderRepository )
		{
			_cfg = configuration; _repo = orderRepository;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var conf = new ConsumerConfig
			{
				BootstrapServers = _cfg["Kafka:BootstrapServers"],
				GroupId = "order-consumers",
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = false
			};

			using var c = new ConsumerBuilder<string, string>(conf).Build();
			c.Subscribe(new[]{ _cfg["Kafka:Topics:InventoryReserved"] ?? "inventory.reserved",
						   _cfg["Kafka:Topics:InventoryFailed"] ?? "inventory.failed" });

			while (!stoppingToken.IsCancellationRequested) {

				var cr = c.Consume(TimeSpan.FromMilliseconds(500));
				if (cr is null) continue;

				try
				{



					if (cr.Topic.EndsWith("inventory.reserved"))
					{
						var evt = JsonSerializer.Deserialize<InventoryReserved>(cr.Message.Value)!;
						await _repo.UpdateStatusAsync(Guid.Parse(evt.OrderId), "Confirmed");
					}
					else if (cr.Topic.EndsWith("inventory.failed")) {
						var evt = JsonSerializer.Deserialize<InventoryReserved>(cr.Message.Value)!;
						await _repo.UpdateStatusAsync(Guid.Parse(evt.OrderId), "Rejected");

					}

					c.Commit(cr);
				}
				catch 
				{

					
				}
			}
		}
	}
}
