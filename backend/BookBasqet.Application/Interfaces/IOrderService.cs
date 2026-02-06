using BookBasqet.Application.DTOs.Orders;

namespace BookBasqet.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(int userId);
    Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> UpdateStatusAsync(int orderId, string status);
}
