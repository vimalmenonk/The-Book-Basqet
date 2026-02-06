namespace BookBasqet.Application.Models.Email;

public class OrderEmailItemModel
{
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
