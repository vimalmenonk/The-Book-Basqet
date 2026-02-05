using BookBasqet.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookBasqet.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _hasher = new();

    public string HashPassword(string password) => _hasher.HashPassword(new object(), password);

    public bool VerifyPassword(string password, string passwordHash)
        => _hasher.VerifyHashedPassword(new object(), passwordHash, password) != PasswordVerificationResult.Failed;
}
