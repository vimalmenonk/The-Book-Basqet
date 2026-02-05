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

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
        => Ok(ApiResponse<OrderDto>.Ok(await _service.CheckoutAsync(User.GetUserId()), "Order placed"));

    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
        => Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(await _service.GetMyOrdersAsync(User.GetUserId())));

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(await _service.GetAllOrdersAsync()));
}
