namespace BookBasqet.Domain.Entities;

public class CartItem : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public int Quantity { get; set; }
}
