namespace BookBasqet.Application.Models.Email;

public class OrderEmailModel
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyCollection<OrderEmailItemModel> Items { get; set; } = Array.Empty<OrderEmailItemModel>();
}
