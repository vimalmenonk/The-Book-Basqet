namespace BookBasqet.Application.DTOs.Orders;

public class CreateOrderDto { }

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class OrderItemDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

public class OrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}
