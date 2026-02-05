using BookBasqet.Application.Models.Email;

namespace BookBasqet.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendOrderConfirmationAsync(OrderEmailModel model, CancellationToken cancellationToken = default);
    Task<bool> SendAdminOrderAlertAsync(OrderEmailModel model, CancellationToken cancellationToken = default);
}
