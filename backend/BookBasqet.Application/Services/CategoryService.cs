using BookBasqet.Application.DTOs.Categories;
using BookBasqet.Application.Interfaces;
using BookBasqet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookBasqet.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync() => await _context.Categories
        .Select(x => new CategoryDto { Id = x.Id, Name = x.Name, Description = x.Description })
        .ToListAsync();

    public async Task<CategoryDto?> GetByIdAsync(int id) => await _context.Categories
        .Where(x => x.Id == id)
        .Select(x => new CategoryDto { Id = x.Id, Name = x.Name, Description = x.Description })
        .FirstOrDefaultAsync();

    public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
    {
        var entity = new Category { Name = dto.Name, Description = dto.Description };
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return new CategoryDto { Id = entity.Id, Name = entity.Name, Description = entity.Description };
    }

    public async Task<CategoryDto?> UpdateAsync(int id, CategoryUpdateDto dto)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity is null) return null;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        await _context.SaveChangesAsync();
        return new CategoryDto { Id = entity.Id, Name = entity.Name, Description = entity.Description };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity is null) return false;
        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
