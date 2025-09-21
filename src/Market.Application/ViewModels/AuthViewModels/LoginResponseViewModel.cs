using Market.Application.ViewModels.UserViewModels;

namespace Market.Application.ViewModels.AuthViewModels;

public class LoginResponseViewModel
{
    public required string AccessToken { get; set; }
    public required ListUserViewModel User { get; set; }
}