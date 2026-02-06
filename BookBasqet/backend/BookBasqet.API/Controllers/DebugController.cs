using BookBasqet.Infrastructure.Persistence;
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
                Role = x.Role != null ? x.Role.Name : "Unknown",
                PasswordHashPreview = x.PasswordHash.Length > 20 ? $"{x.PasswordHash[..20]}..." : x.PasswordHash
            })
            .ToListAsync();

        return Ok(users);
    }
}
