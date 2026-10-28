using AssetPass.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AssetPass.Infrastructure.Services;

/// <summary>
/// Mock email service that logs emails to console.
/// Replace with a real implementation (SendGrid, SES, etc.) in production.
/// </summary>
public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK EMAIL] To: {To} | Subject: {Subject} | Body length: {BodyLength}",
            to, subject, htmlBody.Length);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK EMAIL] Password Reset | To: {To} | Link: {Link}",
            to, resetLink);
        return Task.CompletedTask;
    }

    public Task SendInvitationEmailAsync(string to, string inviterName, string companyName, string acceptLink, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK EMAIL] Invitation | To: {To} | From: {Inviter} | Company: {Company} | Link: {Link}",
            to, inviterName, companyName, acceptLink);
        return Task.CompletedTask;
    }
}
