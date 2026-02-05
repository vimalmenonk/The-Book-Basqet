using BookBasqet.Application.DTOs.Books;
using BookBasqet.Application.Interfaces;
using BookBasqet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Application.Services;

public class BookService : IBookService
{
    private readonly IApplicationDbContext _context;

    public BookService(IApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<BookDto>> GetAllAsync() => await _context.Books.Include(x => x.Category)
        .Select(x => ToDto(x))
        .ToListAsync();

    public async Task<BookDto?> GetByIdAsync(int id) => await _context.Books.Include(x => x.Category)
        .Where(x => x.Id == id).Select(x => ToDto(x)).FirstOrDefaultAsync();

    public async Task<BookDto> CreateAsync(BookCreateDto dto)
    {
        var categoryExists = await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId);
        if (!categoryExists) throw new InvalidOperationException("Category does not exist.");

        var entity = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            Isbn = dto.Isbn,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CoverImageUrl = dto.CoverImageUrl,
            CategoryId = dto.CategoryId
        };
        _context.Books.Add(entity);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task<BookDto?> UpdateAsync(int id, BookUpdateDto dto)
    {
        var entity = await _context.Books.FindAsync(id);
        if (entity is null) return null;

        entity.Title = dto.Title;
        entity.Author = dto.Author;
        entity.Isbn = dto.Isbn;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.StockQuantity = dto.StockQuantity;
        entity.CoverImageUrl = dto.CoverImageUrl;
        entity.CategoryId = dto.CategoryId;
        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Books.FindAsync(id);
        if (entity is null) return false;
        _context.Books.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static BookDto ToDto(Book x) => new()
    {
        Id = x.Id,
        Title = x.Title,
        Author = x.Author,
        Isbn = x.Isbn,
        Description = x.Description,
        Price = x.Price,
        StockQuantity = x.StockQuantity,
        CoverImageUrl = x.CoverImageUrl,
        CategoryId = x.CategoryId,
        CategoryName = x.Category != null ? x.Category.Name : string.Empty
    };
}
