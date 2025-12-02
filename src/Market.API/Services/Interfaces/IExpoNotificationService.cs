namespace Market.API.Services.Interfaces;

public interface IExpoNotificationService
{
    Task SendNotificationAsync(string pushToken, string title, string message, object? data = null, CancellationToken cancellationToken = default);
}