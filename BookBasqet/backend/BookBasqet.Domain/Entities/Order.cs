namespace BookBasqet.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
