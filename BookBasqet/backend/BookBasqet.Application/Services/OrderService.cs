using BookBasqet.Application.DTOs.Orders;
using BookBasqet.Application.Interfaces;
using BookBasqet.Application.Models.Email;
using BookBasqet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Application.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public OrderService(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<OrderDto> CheckoutAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Include(x => x.Book)
            .Where(x => x.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            throw new InvalidOperationException("Cart is empty.");

        foreach (var item in cartItems)
        {
            if (item.Book!.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {item.Book.Title}");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId)
                   ?? throw new InvalidOperationException("User not found.");

        var order = new Order { UserId = userId, Status = "Pending" };

        foreach (var item in cartItems)
        {
            item.Book!.StockQuantity -= item.Quantity;
            order.Items.Add(new OrderItem
            {
                BookId = item.BookId,
                Quantity = item.Quantity,
                UnitPrice = item.Book.Price
            });
        }

        order.TotalAmount = order.Items.Sum(x => x.Quantity * x.UnitPrice);
        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync();

        await TrySendOrderEmailsAsync(order, user, cartItems);

        return await MapOrderAsync(order.Id) ?? throw new InvalidOperationException("Order creation failed.");
    }

    public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId)
        => await _context.Orders.Where(x => x.UserId == userId).Select(ProjectOrder()).ToListAsync();

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        => await _context.Orders.Select(ProjectOrder()).ToListAsync();

    public async Task<OrderDto?> UpdateStatusAsync(int orderId, string status)
    {
        var allowedStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Pending", "Shipped", "Delivered" };
        if (!allowedStatuses.Contains(status))
            throw new InvalidOperationException("Invalid order status. Allowed values are Pending, Shipped, Delivered.");

        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order is null) return null;

        order.Status = allowedStatuses.First(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
        await _context.SaveChangesAsync();

        return await MapOrderAsync(orderId);
    }

    private async Task TrySendOrderEmailsAsync(Order order, User user, IReadOnlyCollection<CartItem> cartItems)
    {
        var model = new OrderEmailModel
        {
            OrderId = order.Id,
            CustomerName = user.Name,
            CustomerEmail = user.Email,
            CreatedAt = order.CreatedAt,
            TotalAmount = order.TotalAmount,
            Items = cartItems.Select(i => new OrderEmailItemModel
            {
                Title = i.Book?.Title ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.Book?.Price ?? 0
            }).ToList()
        };

        await _emailService.SendOrderConfirmationAsync(model);
        await _emailService.SendAdminOrderAlertAsync(model);
    }

    private async Task<OrderDto?> MapOrderAsync(int orderId)
        => await _context.Orders.Where(x => x.Id == orderId).Select(ProjectOrder()).FirstOrDefaultAsync();

    private static System.Linq.Expressions.Expression<Func<Order, OrderDto>> ProjectOrder() => order => new OrderDto
    {
        Id = order.Id,
        CreatedAt = order.CreatedAt,
        Status = order.Status,
        TotalAmount = order.TotalAmount,
        Items = order.Items.Select(i => new OrderItemDto
        {
            BookId = i.BookId,
            Title = i.Book != null ? i.Book.Title : string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };
}
