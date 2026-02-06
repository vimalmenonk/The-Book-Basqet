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
        await context.Database.EnsureCreatedAsync();

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category
                {
                    Id = 1,
                    Name = "Fiction",
                    Description = "Fictional stories",
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = 2,
                    Name = "Technology",
                    Description = "Tech books",
                    CreatedAt = DateTime.UtcNow
                });
        }

        if (!await context.Books.AnyAsync())
        {
            context.Books.AddRange(
                new Book
                {
                    Id = 1,
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    Isbn = "9780132350884",
                    Description = "A handbook of agile software craftsmanship.",
                    Price = 34.99m,
                    StockQuantity = 20,
                    CoverImageUrl = "https://placehold.co/240x320",
                    CategoryId = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new Book
                {
                    Id = 2,
                    Title = "The Alchemist",
                    Author = "Paulo Coelho",
                    Isbn = "9780061122415",
                    Description = "A novel about following dreams.",
                    Price = 14.99m,
                    StockQuantity = 50,
                    CoverImageUrl = "https://placehold.co/240x320",
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow
                });
        }

        if (!await context.Users.AnyAsync())
        {
            var hasher = new PasswordHasher();

            context.Users.AddRange(
                new User
                {
                    Name = "System Admin",
                    Email = "admin@bookbasqet.com",
                    PasswordHash = hasher.HashPassword("Admin@123"),
                    RoleId = (int)RoleType.Admin,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Name = "Default User",
                    Email = "user@bookbasqet.com",
                    PasswordHash = hasher.HashPassword("User@123"),
                    RoleId = (int)RoleType.User,
                    CreatedAt = DateTime.UtcNow
                });
        }

        await context.SaveChangesAsync();
    }
}
