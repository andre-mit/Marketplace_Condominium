using Market.SharedApplication.ViewModels.UserViewModels;

namespace Market.SharedApplication.ViewModels.AuthViewModels;

public class LoginResponseViewModel
{
    public required string AccessToken { get; set; }
    public required ListUserViewModel User { get; set; }
}