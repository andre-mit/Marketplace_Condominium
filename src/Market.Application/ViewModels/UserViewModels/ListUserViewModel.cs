using Market.Domain.Entities;

namespace Market.SharedApplication.ViewModels.UserViewModels;

public class ListUserViewModel
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    public required string Email { get; set; }

    public required DateOnly Birth { get; set; }
    public required string Unit { get; set; }
    public required string Tower { get; set; }
    
    public string? AvatarUrl { get; set; } = null;
    
    public required List<Product> Products { get; set; }

    public static implicit operator ListUserViewModel(User user) => ParseFromUser(user);

    private static ListUserViewModel ParseFromUser(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Birth = user.Birth,
        Unit = user.Unit,
        Tower = user.Tower,
        AvatarUrl = user.AvatarUrl,
        Products = user.Products?.ToList() ?? []
    };
}