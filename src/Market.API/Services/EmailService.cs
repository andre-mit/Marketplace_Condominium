using Market.API.SettingsModels;
using Microsoft.Extensions.Options;
using Resend;

namespace Market.API.Services;

public class EmailService(ILogger<EmailService> logger, IResend resend, EmailOptions _emailOptions) : IEmailService
{

    public async Task SendVerificationEmailAsync(string toEmail, string verificationCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = _emailOptions.From,
                HtmlBody = $"<p>Your verification code is: <strong>{verificationCode}</strong></p>",
                Subject = "Verificação de Email"
            };

            email.To.Add(toEmail);
            await resend.EmailSendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending verification email to {ToEmail}", toEmail);
            throw;
        }
    }
    
    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = _emailOptions.From,
                HtmlBody = $"<p>Click <a href=\"{resetLink}\">here</a> to reset your password.</p>",
                Subject = "Password Reset"
            };

            email.To.Add(toEmail);
            await resend.EmailSendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending password reset email to {ToEmail}", toEmail);
            throw;
        }
    }
    
    public async Task SendVerifiedNotificationEmailAsync(string toEmail,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = _emailOptions.From,
                HtmlBody = "<p>Your account has been successfully verified.</p>",
                Subject = "Account Verified"
            };

            email.To.Add(toEmail);
            await resend.EmailSendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending verified notification email to {ToEmail}", toEmail);
            throw;
        }
    }
    
    public async Task SendManualReviewNotificationEmailAsync(string toEmail,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = _emailOptions.From,
                HtmlBody = "<p>Your account is pending manual review. We will notify you once the review is complete.</p>",
                Subject = "Account Pending Manual Review"
            };

            email.To.Add(toEmail);
            await resend.EmailSendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending manual review notification email to {ToEmail}", toEmail);
            throw;
        }
    }
    
    public async Task SendWelcomeEmailAsync(string toEmail,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = _emailOptions.From,
                HtmlBody = "<p>Welcome to our platform! We're excited to have you on board.</p>",
                Subject = "Welcome to Our Platform"
            };

            email.To.Add(toEmail);
            await resend.EmailSendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending welcome email to {ToEmail}", toEmail);
            throw;
        }
    }
    
    public async Task SendGoodbyeEmailAsync(string toEmail,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = _emailOptions.From,
                HtmlBody = "<p>We're sorry to see you go. If you have any feedback, please let us know.</p>",
                Subject = "Goodbye from Our Platform"
            };

            email.To.Add(toEmail);
            await resend.EmailSendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending goodbye email to {ToEmail}", toEmail);
            throw;
        }
    }
    
    public async Task SendBannedNotificationEmailAsync(string toEmail, DateTime dateUntil,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = _emailOptions.From,
                HtmlBody = $"<p>Your account has been banned until {dateUntil:D}. Please contact support for more information.</p>",
                Subject = "Account Banned Notification"
            };

            email.To.Add(toEmail);
            await resend.EmailSendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending banned notification email to {ToEmail}", toEmail);
            throw;
        }
    }
}