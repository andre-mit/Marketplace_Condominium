using Market.API.Hubs;
using Market.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Market.API.Services;

public class UserVerificationService(
    ILogger<UserVerificationService> logger,
    IHubContext<AdminHub> adminHub,
    IHttpClientFactory httpClientFactory,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("UserConfirmationApi");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("UserVerificationService started at: {time}", DateTimeOffset.Now);

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var workServiceScope = serviceProvider.CreateScope();
            var usersRepository = workServiceScope.ServiceProvider.GetRequiredService<IUsersRepository>();
            try
            {
                var usersToVerify = await usersRepository.GetUsersByStatusAsync(
                    UserVerificationStatus.Pending, stoppingToken);

                foreach (var user in usersToVerify)
                {
                    await VerifyUserAsync(user, stoppingToken);
                    await Task.Delay(500, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in UserVerificationService");
            }
        }
    }

    private async Task VerifyUserAsync(User user,
        CancellationToken cancellationToken = default)
    {
        var success = false;
        var count = 0;

        using var workServiceScope = serviceProvider.CreateScope();
        var usersRepository = workServiceScope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var unitOfWork = workServiceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var emailService = workServiceScope.ServiceProvider.GetRequiredService<IEmailService>();

        do
        {
            var jsonData = JsonConvert.SerializeObject(new UserVerificationRequest(user.FullName, user.Cpf, user.Birth,
                user.Email, user.Phone, user.Unit, user.Tower));

            var jsonDataContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/users/verify", jsonDataContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("User {Name} with CPF {Cpf} verified successfully", user.FullName, user.Cpf);
                await usersRepository.UpdateStatusAsync(user.Id, UserVerificationStatus.Verified,
                    cancellationToken);

                try
                {
                    await unitOfWork.CommitAsync(cancellationToken);
                    success = true;

                    await emailService.SendVerifiedNotificationEmailAsync(user.Email, cancellationToken);
                    logger.LogInformation("Sent user verified email to {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error committing user verification status for user {Name} with CPF {Cpf}",
                        user.FullName, user.Cpf);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                logger.LogWarning("User {Name} with CPF {Cpf} verification failed", user.FullName, user.Cpf);

                await usersRepository.UpdateStatusAsync(user.Id, UserVerificationStatus.PendingManualReview,
                    cancellationToken);
                await adminHub.Clients.All.SendAsync("UserPendingManualReview", new { UserId = user.Id },
                    cancellationToken);

                success = true;
            }
            else
            {
                logger.LogError("User {Name} with CPF {Cpf} verification failed", user.FullName, user.Cpf);
            }

            count++;
        } while (!success && count < 3 && !cancellationToken.IsCancellationRequested);
    }

    private record UserVerificationRequest(
        string Name,
        string Cpf,
        DateOnly Birth,
        string Email,
        string Phone,
        string Unit,
        string Tower);
}