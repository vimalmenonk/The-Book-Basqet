using System.ComponentModel.DataAnnotations;

namespace BookBasqet.Application.DTOs.Categories;

public class CategoryCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}

public class CategoryUpdateDto : CategoryCreateDto { }

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
