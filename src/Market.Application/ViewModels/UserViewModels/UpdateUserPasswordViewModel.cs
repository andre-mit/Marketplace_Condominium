using System.ComponentModel.DataAnnotations;

namespace Market.SharedApplication.ViewModels.UserViewModels;

public class UpdateUserPasswordViewModel
{
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }
}