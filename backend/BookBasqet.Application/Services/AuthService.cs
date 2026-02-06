using BookBasqet.Application.DTOs.Auth;
using BookBasqet.Application.Interfaces;
using BookBasqet.Domain.Entities;
using BookBasqet.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IApplicationDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(x => x.Email == email))
            throw new InvalidOperationException("Email is already registered.");

        var role = await _context.Roles.FirstAsync(x => x.Id == (int)RoleType.User);
        var user = new User
        {
            Name = request.Name,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            RoleId = role.Id
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var (token, expiresAt) = _tokenService.GenerateToken(user, role.Name);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Name = user.Name,
            Email = user.Email,
            Role = role.Name
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roleName = user.Role?.Name ?? "User";
        var (token, expiresAt) = _tokenService.GenerateToken(user, roleName);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Name = user.Name,
            Email = user.Email,
            Role = roleName
        };
    }
}
