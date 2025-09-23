using Microsoft.Data.SqlClient.Diagnostics;

namespace Order.API.Services
{
	public class ProductClient
	{
		private readonly HttpClient _http;

		public ProductClient( HttpClient http) => _http = http;


		private record ProductVm(string Sku, string Name, decimal Price, bool IsActive);


		public async Task<decimal?> GetPriceAsync(string sku, CancellationToken ct = default)
		{
			try
			{
				var vm = await _http.GetFromJsonAsync<ProductVm>($"api/product/{sku}", ct);
				return vm?.IsActive == true ? vm.Price : null;
			}
			catch { return null; }
		}

	}
}
