using Market.API.Hubs;
using Market.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Market.API.Services;

public class UserVerificationService : BackgroundService
{
    private readonly HttpClient _client;
    
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserVerificationService> _logger;
    private readonly IHubContext<AdminHub> _adminHub;

    public UserVerificationService(
        ILogger<UserVerificationService> logger,
        IHubContext<AdminHub> adminHub,
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory serviceScopeFactory)
    {
        _client = httpClientFactory.CreateClient("UserConfirmationApi");
        
        _logger = logger;
        _adminHub = adminHub;
        
        using var scope = serviceScopeFactory.CreateScope();
        _usersRepository =
            scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        
        _logger.LogInformation("UserVerificationService started at: {time}", DateTimeOffset.Now);

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var usersToVerify = await _usersRepository.GetUsersByStatusAsync(
                    UserVerificationStatus.Pending, stoppingToken);

                foreach (var user in usersToVerify)
                {
                    await VerifyUserAsync(user.Id, user.FullName, user.Cpf, user.Email, stoppingToken);
                    await Task.Delay(500, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserVerificationService");
            }
        }
    }

    private async Task VerifyUserAsync(Guid userId, string name, string cpf, string email,
        CancellationToken cancellationToken = default)
    {
        var success = false;
        var count = 0;
        do
        {
            var jsonData = JsonConvert.SerializeObject(new UserVerificationRequest(name, cpf));

            var jsonDataContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/users/verify", jsonDataContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var isVerified = JsonConvert.DeserializeObject<bool>(responseContent);

                if (isVerified)
                {
                    _logger.LogInformation("User {Name} with CPF {Cpf} verified successfully", name, cpf);
                    await _usersRepository.UpdateStatusAsync(userId, UserVerificationStatus.Verified, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("User {Name} with CPF {Cpf} verification failed", name, cpf);

                    await _usersRepository.UpdateStatusAsync(userId, UserVerificationStatus.PendingManualReview,
                        cancellationToken);
                    await _adminHub.Clients.All.SendAsync("UserPendingManualReview", new { UserId = userId },
                        cancellationToken);
                }

                try
                {
                    await _unitOfWork.CommitAsync(cancellationToken);
                    success = true;

                    await _emailService.SendVerifiedNotificationEmailAsync(email, cancellationToken);
                    _logger.LogInformation("Sent user verified email to {Email}", email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error committing user verification status for user {Name} with CPF {Cpf}",
                        name,
                        cpf);
                }
            }
            else
            {
                _logger.LogError("Error verifying user {Name} with CPF {Cpf}: {StatusCode}", name, cpf,
                    response.StatusCode);
            }

            count++;
        } while (!success && count < 3 && !cancellationToken.IsCancellationRequested);
    }

    private record UserVerificationRequest(string Name, string Cpf);
}