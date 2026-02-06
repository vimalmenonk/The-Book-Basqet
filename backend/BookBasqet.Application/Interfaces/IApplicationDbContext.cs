using BookBasqet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Book> Books { get; }
    DbSet<Category> Categories { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
