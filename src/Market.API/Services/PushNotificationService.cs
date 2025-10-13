using FirebaseAdmin;
using Market.API.Services.Interfaces;

namespace Market.API.Services;

public class PushNotificationService : IPushNotificationService
{
    private readonly ILogger<PushNotificationService> _logger;
    public PushNotificationService(ILogger<PushNotificationService> logger)
    {
        _logger = logger;
        
        FirebaseApp.Create(new AppOptions
        {
            Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("firebase-adminsdk.json")
        });
    }
    
    public async Task SendNotificationAsync(string title, string body, string token, Dictionary<string, string> data, string collapseKey, CancellationToken cancellationToken = default)
    {
        var message = new FirebaseAdmin.Messaging.Message()
        {
            Token = token,
            Android = new FirebaseAdmin.Messaging.AndroidConfig
            {
                Notification = new FirebaseAdmin.Messaging.AndroidNotification()
                {
                    Title = title,
                    Body = body
                },
                CollapseKey = collapseKey,
                Data = data
            }
        };
        
        var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
        
        _logger.LogInformation("Sent push notification with response: {Response}", response);
    }
}