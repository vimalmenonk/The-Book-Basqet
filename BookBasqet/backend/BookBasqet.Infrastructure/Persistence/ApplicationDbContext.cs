using BookBasqet.Application.Interfaces;
using BookBasqet.Domain.Entities;
using BookBasqet.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Book>().HasIndex(x => x.Isbn).IsUnique();

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = (int)RoleType.User, Name = "User", CreatedAt = DateTime.UtcNow },
            new Role { Id = (int)RoleType.Admin, Name = "Admin", CreatedAt = DateTime.UtcNow });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction", Description = "Fictional stories", CreatedAt = DateTime.UtcNow },
            new Category { Id = 2, Name = "Technology", Description = "Tech books", CreatedAt = DateTime.UtcNow });

        modelBuilder.Entity<Book>().HasData(
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

        modelBuilder.Entity<CartItem>()
            .HasOne(x => x.Book)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(x => x.Book)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
