namespace Market.API.Services.Interfaces;

public interface IPushNotificationService
{
    /// <summary>
    /// Sends a push notification to a device using Firebase Cloud Messaging.
    /// </summary>
    /// <param name="title">The title of the notification.</param>
    /// <param name="body">The body content of the notification.</param>
    /// <param name="token">The device token to which the notification will be sent.</param>
    /// <param name="data">Additional data to include in the notification payload.</param>
    /// <param name="collapseKey">A key that allows FCM to replace old messages with new ones.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendNotificationAsync(string title, string body, string token, Dictionary<string, string> data,
        string collapseKey, CancellationToken cancellationToken = default);
}