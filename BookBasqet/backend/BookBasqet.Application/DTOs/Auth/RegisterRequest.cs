using System.ComponentModel.DataAnnotations;

namespace BookBasqet.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
