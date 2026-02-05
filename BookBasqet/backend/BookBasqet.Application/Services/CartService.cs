using BookBasqet.Application.DTOs.Cart;
using BookBasqet.Application.Interfaces;
using BookBasqet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Application.Services;

public class CartService : ICartService
{
    private readonly IApplicationDbContext _context;

    public CartService(IApplicationDbContext context) => _context = context;

    public async Task<CartDto> GetMyCartAsync(int userId)
    {
        var items = await _context.CartItems.Include(x => x.Book)
            .Where(x => x.UserId == userId)
            .Select(x => new CartItemDto
            {
                Id = x.Id,
                BookId = x.BookId,
                Title = x.Book!.Title,
                UnitPrice = x.Book!.Price,
                Quantity = x.Quantity
            }).ToListAsync();

        return new CartDto { Items = items };
    }

    public async Task<CartDto> AddItemAsync(int userId, AddCartItemDto dto)
    {
        var book = await _context.Books.FindAsync(dto.BookId) ?? throw new InvalidOperationException("Book not found.");
        if (book.StockQuantity < dto.Quantity) throw new InvalidOperationException("Requested quantity exceeds stock.");

        var existing = await _context.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == dto.BookId);
        if (existing is null)
            _context.CartItems.Add(new CartItem { UserId = userId, BookId = dto.BookId, Quantity = dto.Quantity });
        else
            existing.Quantity += dto.Quantity;

        await _context.SaveChangesAsync();
        return await GetMyCartAsync(userId);
    }

    public async Task<CartDto?> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
    {
        var item = await _context.CartItems.Include(x => x.Book).FirstOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId);
        if (item is null) return null;
        if (item.Book!.StockQuantity < dto.Quantity) throw new InvalidOperationException("Requested quantity exceeds stock.");
        item.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();
        return await GetMyCartAsync(userId);
    }

    public async Task<bool> RemoveItemAsync(int userId, int cartItemId)
    {
        var item = await _context.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId);
        if (item is null) return false;
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
