using System.ComponentModel.DataAnnotations;

namespace Market.SharedApplication.ViewModels.UserViewModels;

public class CreateUserViewModel<T>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF must have 11 digits")]
    [RegularExpression(@"^\d+$", ErrorMessage = "CPF must contain only numbers")] 
    public required string Cpf { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone number")]
    public required string Phone { get; set; }
    
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }

    public required DateTime Birth { get; set; }
    
    [MaxLength(10, ErrorMessage = "Unit must be at most 10 characters long")]
    public required string Unit { get; set; }
    
    [MaxLength(10, ErrorMessage = "Tower must be at most 10 characters long")]
    public required string Tower { get; set; }
    
    public T? Image { get; set; }
}