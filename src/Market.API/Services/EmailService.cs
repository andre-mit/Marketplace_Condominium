using Market.API.SettingsModels;
using Microsoft.Extensions.Options;
using Resend;

namespace Market.API.Services;

public class EmailService(ILogger<EmailService> logger, IResend resend, EmailOptions emailOptions) : IEmailService
{

    public async Task SendVerificationEmailAsync(string toEmail, string verificationCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = emailOptions.From,
                HtmlBody = $"<p>Use o código <b>${verificationCode}</b> para verificar seu email</p>",
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
    
    public async Task SendPasswordResetEmailAsync(string toEmail, string code,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new EmailMessage
            {
                From = emailOptions.From,
                HtmlBody = $"<p>Use o código <b>${code}</b> para restaurar sua senha</p>",
                Subject = "Esqueci minha senha"
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
                From = emailOptions.From,
                HtmlBody = "<p>Sua conta foi verificada com sucesso. Você ja pode acessar o Marketplace do seu condomínio</p>",
                Subject = "Conta verificada"
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
                From = emailOptions.From,
                HtmlBody = "<p>Sua conta está pendente de revisão manual. Pode ser que entremos em contato via email ou telefone para confirmar informações.</p>",
                Subject = "Conta pendente de revisão manual"
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
                From = emailOptions.From,
                HtmlBody = "<p>Bem-vindo à nossa plataforma! Estamos felizes em tê-lo conosco.</p>",
                Subject = "Bem-vindo à nossa plataforma de Marketplace"
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
                From = emailOptions.From,
                HtmlBody = "<p>Lamentamos vê-lo partir. Se mudar de ideia, estaremos aqui para recebê-lo de volta.</p>",
                Subject = "Sentimos sua falta na nossa plataforma de Marketplace"
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
                From = emailOptions.From,
                HtmlBody = $"<p>Sua conta foi banida até {dateUntil:dd/MM/yyyy}. Durante esse período, você não poderá acessar nossos serviços.</p>",
                Subject = "Conta banida na nossa plataforma de Marketplace"
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