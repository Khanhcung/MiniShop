using BuildingBlocks.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Order.API.Dtos;
using Order.API.Repositories;
using Order.API.Services;
using Shared.Contracts;
using System.Text.Json;

namespace Order.API.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IOrderRepository _repo;
		private readonly IDistributedCache _cache;
		private readonly IKafkaProducer _producer;
		private readonly ProductClient _products;


		public OrderController(IOrderRepository repository,IDistributedCache cache,ProductClient client,IKafkaProducer producer)
		{
			_repo = repository;
			_cache = cache;
			_producer = producer;
			_products = client;
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateOrderDto dto, CancellationToken ct)
		{
			var linesResolved = new List<(string sku, int qty, decimal unitPrice)>();
			foreach (var l in dto.Lines)
			{
				decimal price;
				var cached = await _cache.GetStringAsync($"product:{l.Sku}:price", ct);
				if (cached is null)
				{
					var p = await _products.GetPriceAsync(l.Sku, ct);
					if (p is null) return BadRequest(new { error = $"SKU_NOT_FOUND_OR_INACTIVE: {l.Sku}" });
					price = p.Value;
					await _cache.SetStringAsync($"product:{l.Sku}:price", price.ToString(),
						new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, ct);
				}
				else price = decimal.Parse(cached);

				linesResolved.Add((l.Sku, l.Qty, price));
			}

			var total = linesResolved.Sum(x =>x.qty * x.unitPrice);

			var code = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
			var orderId = await _repo.CreateAsync(code, linesResolved, total);


			var evt = new OrderCreated(orderId.ToString(), code,
		   linesResolved.Select(x => new OrderLine(x.sku, x.qty, x.unitPrice)).ToList(), DateTime.UtcNow);
			await _producer.ProduceAsync("order.created", orderId.ToString(), JsonSerializer.Serialize(evt));

			return Ok(new { orderId, code, total });
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<OrderVm>> Get(Guid id)
		{
			var vm = await _repo.GetAsync(id);
			return vm is null ? NotFound() : Ok(vm);
		}

		[HttpPost("{id:guid}/cancel")]
		public async Task<IActionResult> Cancel(Guid id)
		{
			await _repo.UpdateStatusAsync(id, "Canceled");
			// TODO: publish order.canceled for release reserved
			return NoContent();
		}

	}
}
