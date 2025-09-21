namespace Market.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    public required string CPF { get; set; }
    
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }

    public required DateOnly Birth { get; set; }
    public required string Unit { get; set; }
    public required string Tower { get; set; }

    public List<Role>? Roles { get; set; }
    
    public List<Product>? Products { get; set; }
    public List<Sale>? Sales { get; set; }
    public List<Sale>? Purchases { get; set; }
    
    public List<ChatSession>? ChatSellerSessions { get; set; }
    public List<ChatSession>? ChatCustomerSessions { get; set; }
}