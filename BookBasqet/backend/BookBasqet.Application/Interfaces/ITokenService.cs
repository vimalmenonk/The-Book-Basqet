using BookBasqet.Domain.Entities;

namespace BookBasqet.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user, string roleName);
}
