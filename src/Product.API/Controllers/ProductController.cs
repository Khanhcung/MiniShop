using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Product.API.Dtos;
using Product.API.Repositories;

namespace Product.API.Controllers
{

	[ApiController]
	[Route("api/[controller]")]

	public class ProductController : ControllerBase
	{
		private readonly IProductRepository _repo;
		private readonly IDistributedCache _cache;

		public ProductController(IProductRepository repo, IDistributedCache cache)
		{
			_repo = repo;
			_cache = cache;
		}

		[HttpPost]

		public async Task<IActionResult> Create(CreateProductDto dto)
		{
			await _repo.CreateAsync(dto);

			await _cache.SetStringAsync($"product:{dto.Sku}:price",dto.Price.ToString(), 
				new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3) });

			return CreatedAtAction(nameof(Get), new { sku = dto.Sku }, new { dto.Sku });
		}

		[HttpGet("{sku}")]
		public async Task<ActionResult<ProductVm>> Get(string sku)
		{
			var p = await _repo.GetAsync(sku);
			return p is null ? NotFound() : Ok(p);
		}

		[HttpPut("{sku}")]
		public async Task<IActionResult> Update(string sku, UpdateProductDto dto)
		{
			await _repo.UpdateAsync(sku, dto);
			await _cache.SetStringAsync($"product:{sku}:price", dto.Price.ToString(),
				new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3) });
			return NoContent();
		}

		[HttpDelete("{sku}")]
		public async Task<IActionResult> Delete(string sku)
		{
			await _repo.DeleteAsync(sku);
			await _cache.RemoveAsync($"product:{sku}:price");
			return NoContent();
		}

	}

}
