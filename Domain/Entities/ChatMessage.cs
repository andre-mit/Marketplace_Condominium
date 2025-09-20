namespace Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public required ChatSession ChatSession { get; set; }
    public required User Sender { get; set; }
    public required string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
}