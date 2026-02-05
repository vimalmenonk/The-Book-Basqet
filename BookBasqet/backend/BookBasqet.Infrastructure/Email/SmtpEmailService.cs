using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using BookBasqet.Application.Models.Email;
using BookBasqet.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookBasqet.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpEmailOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpEmailOptions> options, ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<bool> SendOrderConfirmationAsync(OrderEmailModel model, CancellationToken cancellationToken = default)
    {
        var htmlBody = RenderTemplate("order-confirmation.html", model);
        return SendEmailAsync(model.CustomerEmail, $"Order Confirmation #{model.OrderId}", htmlBody, cancellationToken);
    }

    public Task<bool> SendAdminOrderAlertAsync(OrderEmailModel model, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.AdminEmail))
        {
            _logger.LogWarning("Admin order alert email skipped because Email:AdminEmail is not configured.");
            return Task.FromResult(false);
        }

        var htmlBody = RenderTemplate("order-admin-alert.html", model);
        return SendEmailAsync(_options.AdminEmail, $"New Order Placed #{model.OrderId}", htmlBody, cancellationToken);
    }

    private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Email delivery is disabled by configuration. Skipping email to {Recipient}", toEmail);
            return false;
        }

        try
        {
            using var message = new MailMessage
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
                From = new MailAddress(_options.SenderEmail, _options.SenderName)
            };
            message.To.Add(toEmail);

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.UseSsl,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            cancellationToken.ThrowIfCancellationRequested();
            await client.SendMailAsync(message);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", toEmail);
            return false;
        }
    }

    private string RenderTemplate(string templateName, OrderEmailModel model)
    {
        var templateDirectory = Path.Combine(AppContext.BaseDirectory, _options.TemplatePath);
        var templatePath = Path.Combine(templateDirectory, templateName);

        if (!File.Exists(templatePath))
        {
            _logger.LogWarning("Email template not found at {TemplatePath}. Falling back to generated content.", templatePath);
            return BuildFallbackTemplate(model);
        }

        var template = File.ReadAllText(templatePath);
        var rows = BuildOrderRows(model);

        return template
            .Replace("{{OrderId}}", model.OrderId.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{{CustomerName}}", WebUtility.HtmlEncode(model.CustomerName), StringComparison.Ordinal)
            .Replace("{{CustomerEmail}}", WebUtility.HtmlEncode(model.CustomerEmail), StringComparison.Ordinal)
            .Replace("{{CreatedAt}}", model.CreatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{{TotalAmount}}", model.TotalAmount.ToString("C", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{{OrderItemsRows}}", rows, StringComparison.Ordinal);
    }

    private static string BuildOrderRows(OrderEmailModel model)
    {
        var builder = new StringBuilder();
        foreach (var item in model.Items)
        {
            builder.Append($"<tr><td>{WebUtility.HtmlEncode(item.Title)}</td><td>{item.Quantity}</td><td>{item.UnitPrice.ToString("C", CultureInfo.InvariantCulture)}</td></tr>");
        }

        return builder.ToString();
    }

    private static string BuildFallbackTemplate(OrderEmailModel model)
    {
        var rows = BuildOrderRows(model);
        return $"<h2>Order #{model.OrderId}</h2><p>Customer: {WebUtility.HtmlEncode(model.CustomerName)} ({WebUtility.HtmlEncode(model.CustomerEmail)})</p><table border='1' cellpadding='6' cellspacing='0'><thead><tr><th>Book</th><th>Qty</th><th>Price</th></tr></thead><tbody>{rows}</tbody></table><p>Total: {model.TotalAmount.ToString("C", CultureInfo.InvariantCulture)}</p>";
    }
}
