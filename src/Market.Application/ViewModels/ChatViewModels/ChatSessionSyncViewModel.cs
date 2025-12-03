namespace Market.SharedApplication.ViewModels.ChatViewModels;

public class ChatSessionSyncViewModel
{
    public Guid Id { get; set; }
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    
    public Guid ParticipantId { get; set; } 
    public required string ParticipantName { get; set; }
    public string? ParticipantAvatarUrl { get; set; }

    public string? LastMessageContent { get; set; }
    public DateTime LastMessageAt { get; set; }
    public int UnreadCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<ChatMessageSyncViewModel> Messages { get; set; } = new();
}