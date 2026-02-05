using BookBasqet.Domain.Entities;
using BookBasqet.Domain.Enums;
using BookBasqet.Infrastructure.Persistence;
using BookBasqet.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync(x => x.Email == "admin@bookbasqet.com"))
        {
            var hasher = new PasswordHasher();
            context.Users.Add(new User
            {
                FullName = "System Admin",
                Email = "admin@bookbasqet.com",
                PasswordHash = hasher.HashPassword("Admin@123"),
                RoleId = (int)RoleType.Admin,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
}
