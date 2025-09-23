namespace Order.API.Dtos
{
	public record CreateOrderLineDto(string Sku, int Qty);
	public record CreateOrderDto(List<CreateOrderLineDto> Lines);
	public record OrderVm(Guid Id, string Code, string Status, decimal TotalAmount, DateTime CreatedAt);


}
