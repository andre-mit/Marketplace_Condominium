namespace Market.API.Services.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string verificationCode,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetEmailAsync(string toEmail, string resetLink,
        CancellationToken cancellationToken = default);

    Task SendVerifiedNotificationEmailAsync(string toEmail,
        CancellationToken cancellationToken = default);

    Task SendManualReviewNotificationEmailAsync(string toEmail,
        CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(string toEmail,
        CancellationToken cancellationToken = default);

    Task SendGoodbyeEmailAsync(string toEmail,
        CancellationToken cancellationToken = default);

    Task SendBannedNotificationEmailAsync(string toEmail, DateTime dateUntil,
        CancellationToken cancellationToken = default);
}