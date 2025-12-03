namespace Market.SharedApplication.ViewModels.ChatViewModels;

public class SyncChatsViewModel
{
    public DateTime ServerTime { get; set; } = DateTime.UtcNow;

    public List<ChatSessionSyncViewModel> Chats { get; set; } = new();
}