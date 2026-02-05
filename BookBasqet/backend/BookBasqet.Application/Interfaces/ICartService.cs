using BookBasqet.Application.DTOs.Cart;

namespace BookBasqet.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetMyCartAsync(int userId);
    Task<CartDto> AddItemAsync(int userId, AddCartItemDto dto);
    Task<CartDto?> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
    Task<bool> RemoveItemAsync(int userId, int cartItemId);
}
