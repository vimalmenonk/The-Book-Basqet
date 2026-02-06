using System.Security.Claims;
using BookBasqet.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.API.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DebugController(ApplicationDbContext context) => _context = context;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Include(x => x.Role)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Email,
                RoleName = x.Role != null ? x.Role.Name : "Unknown",
                x.PasswordHash
            })
            .ToListAsync();

        var response = users.Select(x => new
        {
            x.Id,
            x.Name,
            x.Email,
            Role = x.RoleName,
            PasswordHashPreview = string.IsNullOrEmpty(x.PasswordHash)
                ? string.Empty
                : x.PasswordHash.Length > 20
                    ? x.PasswordHash.Substring(0, 20) + "..."
                    : x.PasswordHash
        });

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });

        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            AuthenticationType = User.Identity?.AuthenticationType,
            Name = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Role = User.FindFirstValue(ClaimTypes.Role),
            Subject = User.FindFirstValue("sub"),
            Claims = claims
        });
    }
}
