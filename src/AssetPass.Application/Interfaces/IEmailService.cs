namespace AssetPass.Application.Interfaces;

/// <summary>
/// Email service interface for decoupled email sending.
/// Mock implementation logs to console in development.
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken cancellationToken = default);
    Task SendInvitationEmailAsync(string to, string inviterName, string companyName, string acceptLink, CancellationToken cancellationToken = default);
}
