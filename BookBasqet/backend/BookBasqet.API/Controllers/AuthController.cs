using BookBasqet.Application.Common;
using BookBasqet.Application.DTOs.Auth;
using BookBasqet.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookBasqet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
        => Ok(ApiResponse<AuthResponse>.Ok(await _authService.RegisterAsync(request), "Registration successful"));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
        => Ok(ApiResponse<AuthResponse>.Ok(await _authService.LoginAsync(request), "Login successful"));
}
