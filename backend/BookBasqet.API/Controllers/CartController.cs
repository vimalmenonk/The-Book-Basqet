using BookBasqet.API.Extensions;
using BookBasqet.Application.Common;
using BookBasqet.Application.DTOs.Cart;
using BookBasqet.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBasqet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetMine() => Ok(ApiResponse<CartDto>.Ok(await _service.GetMyCartAsync(User.GetUserId())));

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddCartItemDto dto)
        => Ok(ApiResponse<CartDto>.Ok(await _service.AddItemAsync(User.GetUserId(), dto), "Item added to cart"));

    [HttpPut("items/{id:int}")]
    public async Task<IActionResult> UpdateItem(int id, UpdateCartItemDto dto)
    {
        var cart = await _service.UpdateItemAsync(User.GetUserId(), id, dto);
        return cart is null ? NotFound(ApiResponse<string>.Fail("Cart item not found")) : Ok(ApiResponse<CartDto>.Ok(cart, "Cart item updated"));
    }

    [HttpDelete("items/{id:int}")]
    public async Task<IActionResult> RemoveItem(int id)
        => await _service.RemoveItemAsync(User.GetUserId(), id) ? Ok(ApiResponse<string>.Ok("Deleted", "Cart item removed")) : NotFound(ApiResponse<string>.Fail("Cart item not found"));
}
