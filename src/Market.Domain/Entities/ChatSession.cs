namespace Domain.Entities;

public class ChatSession
{
    public Guid Id { get; set; }
    public required User Seller { get; set; }
    public required User Customer { get; set; }
    public required Product Product { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}