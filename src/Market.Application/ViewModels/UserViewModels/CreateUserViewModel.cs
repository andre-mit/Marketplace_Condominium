using System.ComponentModel.DataAnnotations;

namespace Market.Application.ViewModels.UserViewModels;

public class CreateUserViewModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    [RegularExpression(@"\d{3}\.\d{3}\.\d{3}-\d{2}", ErrorMessage = "CPF must be in the format XXX.XXX.XXX-XX")]
    public required string CPF { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }
    
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }

    public required DateOnly Birth { get; set; }
    
    [MaxLength(10, ErrorMessage = "Unit must be at most 10 characters long")]
    public required string Unit { get; set; }
    
    [MaxLength(10, ErrorMessage = "Tower must be at most 10 characters long")]
    public required string Tower { get; set; }
}