namespace Market.Domain.Entities;

public class Image
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}