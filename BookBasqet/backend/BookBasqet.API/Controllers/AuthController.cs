using BookBasqet.Application.Common;
using BookBasqet.Application.DTOs.Auth;
using BookBasqet.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookBasqet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
        => Ok(ApiResponse<AuthResponse>.Ok(await _authService.RegisterAsync(request), "Registration successful"));

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(response, "Login successful"));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(ApiResponse<string>.Fail("Invalid email or password."));
        }
    }
}
