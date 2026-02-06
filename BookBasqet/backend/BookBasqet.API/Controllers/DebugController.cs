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
}
