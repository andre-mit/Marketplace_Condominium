using Market.Domain.Entities;

namespace Market.Application.ViewModels.UserViewModels;

public class ListUserViewModel
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public required string CPF { get; set; }
    
    public required string Email { get; set; }

    public required DateOnly Birth { get; set; }
    public required string Unit { get; set; }
    public required string Tower { get; set; }
    
    public static implicit operator ListUserViewModel(User user) => new ListUserViewModel
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        CPF = user.CPF,
        Email = user.Email,
        Birth = user.Birth,
        Unit = user.Unit,
        Tower = user.Tower
    };
}