using System.Text.Json;
using Market.SharedApplication.ViewModels.NotificationViewModels;

namespace Market.API.Services;

public class ExpoNotificationService(ILogger<ExpoNotificationService> logger, IHttpClientFactory httpClientFactory) : IExpoNotificationService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("ExpoPushApi");

    public async Task SendNotificationAsync(string pushToken, string title, string message, object? data = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(pushToken) || !pushToken.StartsWith("ExponentPushToken"))
        {
            logger.LogWarning("Invalid Expo push token: {PushToken}", pushToken);
            return;
        }

        var payload = new ExpoNotificationRequestViewModel
        {
            To = pushToken,
            Title = title,
            Body = message,
            Data = data
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("",content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Failed to send Expo notification. StatusCode: {StatusCode}, Error: {Error}", response.StatusCode, error);
        }
    }
}