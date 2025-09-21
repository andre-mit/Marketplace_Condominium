using System.ComponentModel.DataAnnotations;

namespace Market.Application.ViewModels.AuthViewModels;

public record LoginRequestViewModel([Required] string Email, [Required] string Password);