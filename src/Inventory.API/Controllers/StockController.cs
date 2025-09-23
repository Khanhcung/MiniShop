using Inventory.API.Dtos;
using Inventory.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class StockController : ControllerBase
	{
		private readonly IStockRepository _repo;

		public StockController(IStockRepository repository)
		{
				_repo = repository;
		}

		[HttpPut("{sku}/adjust")]
		public async Task<IActionResult> Adjust(string sku, AdjustDto dto)
		{
			await _repo.UpsertAdjustAsync(sku, dto.DeltaOnHand);
			return Ok(new { sku, adjusted = dto.DeltaOnHand });
		}

		[HttpGet("{sku}")]
		public async Task<ActionResult<StockVm>> Get(string sku)
		{
			var vm = await _repo.GetAsync(sku);
			return vm is null ? NotFound() : Ok(vm);
		}
	}
}
