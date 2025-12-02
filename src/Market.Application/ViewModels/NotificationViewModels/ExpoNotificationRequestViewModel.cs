namespace Market.SharedApplication.ViewModels.NotificationViewModels;

public class ExpoNotificationRequestViewModel
{
    public string To { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public object Data { get; set; }
    public string Sound { get; set; } = "default";
}