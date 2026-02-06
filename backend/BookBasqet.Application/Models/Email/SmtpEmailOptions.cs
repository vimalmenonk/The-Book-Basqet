namespace BookBasqet.Application.Models.Email;

public class SmtpEmailOptions
{
    public const string SectionName = "Email";

    public bool Enabled { get; set; } = false;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string SenderName { get; set; } = "Book Basqet";
    public string SenderEmail { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = "EmailTemplates";
}
