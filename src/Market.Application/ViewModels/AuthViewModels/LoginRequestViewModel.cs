using System.ComponentModel.DataAnnotations;

namespace Market.SharedApplication.ViewModels.AuthViewModels;

public record LoginRequestViewModel([Required] string Email, [Required] string Password);