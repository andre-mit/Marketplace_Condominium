using System.ComponentModel.DataAnnotations;

namespace Market.SharedApplication.ViewModels.AuthViewModels;

public record LoginRequestViewModel([Required] string Identification, [Required] string Password);