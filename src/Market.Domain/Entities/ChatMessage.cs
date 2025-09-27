namespace Market.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    
    public Guid ChatSessionId { get; set; }
    public ChatSession ChatSession { get; set; }
    
    public Guid SenderId { get; set; }
    public User Sender { get; set; }
    
    public required string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}