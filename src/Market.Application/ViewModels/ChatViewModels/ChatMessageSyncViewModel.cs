namespace Market.SharedApplication.ViewModels.ChatViewModels;

public class ChatMessageSyncViewModel
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string? SenderName { get; set; }
    public string Content { get; set; }
    
    public string Status { get; set; } = "sent"; // sent, delivered, read
    public string Type { get; set; } = "text"; // text, image, system
    
    public DateTime CreatedAt { get; set; }
    public int? ProductId { get; set; }
}