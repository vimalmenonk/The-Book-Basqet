using System.ComponentModel.DataAnnotations;

namespace BookBasqet.Application.DTOs.Books;

public class BookCreateDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(120)]
    public string Author { get; set; } = string.Empty;
    [Required, MaxLength(40)]
    public string Isbn { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    [Range(0.01, 999999)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}

public class BookUpdateDto : BookCreateDto { }

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
