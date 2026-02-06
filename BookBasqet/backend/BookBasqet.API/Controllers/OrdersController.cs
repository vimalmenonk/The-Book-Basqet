using BookBasqet.API.Extensions;
using BookBasqet.Application.Common;
using BookBasqet.Application.DTOs.Orders;
using BookBasqet.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBasqet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service) => _service = service;

    [Authorize(Roles = "User")]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
        => Ok(ApiResponse<OrderDto>.Ok(await _service.CheckoutAsync(User.GetUserId()), "Order placed"));

    [Authorize(Roles = "User")]
    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
        => Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(await _service.GetMyOrdersAsync(User.GetUserId())));

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(await _service.GetAllOrdersAsync()));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var order = await _service.UpdateStatusAsync(id, dto.Status);
        return order is null
            ? NotFound(ApiResponse<string>.Fail("Order not found"))
            : Ok(ApiResponse<OrderDto>.Ok(order, "Order status updated"));
    }
}
