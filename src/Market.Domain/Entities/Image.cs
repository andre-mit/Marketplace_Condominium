namespace Market.Domain.Entities;

public class Image
{
    public Guid Id { get; set; }
    public required string BaseUrl { get; set; }
    public required string Path { get; set; }
    
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}