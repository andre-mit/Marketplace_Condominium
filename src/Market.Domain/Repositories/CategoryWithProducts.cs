using Market.Domain.Entities;

namespace Market.Domain.Repositories;

public class CategoryWithProducts
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public List<Product>? Products { get; set; }
}