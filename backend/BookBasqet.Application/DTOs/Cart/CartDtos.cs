using System.ComponentModel.DataAnnotations;

namespace BookBasqet.Application.DTOs.Cart;

public class AddCartItemDto
{
    [Range(1, int.MaxValue)]
    public int BookId { get; set; }
    [Range(1, 100)]
    public int Quantity { get; set; }
}

public class UpdateCartItemDto
{
    [Range(1, 100)]
    public int Quantity { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Total => Items.Sum(x => x.LineTotal);
}
