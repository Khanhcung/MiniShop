namespace Inventory.API.Dtos
{
	public record AdjustDto(int DeltaOnHand);
	public record StockVm(string Sku, int OnHand, int Reserved);
}
