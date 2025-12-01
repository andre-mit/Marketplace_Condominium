using Market.Domain.Entities;

namespace Market.SharedApplication.ViewModels.UserViewModels;

public class ListUserViewModel
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    public required string Email { get; set; }
    
    public string? Phone { get; set; } = null;
    
    public string? Cpf { get; set; } = null;

    public required DateOnly Birth { get; set; }
    public required string Unit { get; set; }
    public required string Tower { get; set; }
    
    public string? AvatarUrl { get; set; } = null;
    
    public decimal Rating { get; set; }
    
    public required List<Product> Products { get; set; }

    public static implicit operator ListUserViewModel(User user) => ParseFromUser(user);

    private static ListUserViewModel ParseFromUser(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Cpf = user.Cpf,
        Birth = user.Birth,
        Unit = user.Unit,
        Tower = user.Tower,
        AvatarUrl = user.AvatarUrl,
        Rating = (decimal)user.Rating / 10,
        Products = user.Products?.ToList() ?? []
    };
}