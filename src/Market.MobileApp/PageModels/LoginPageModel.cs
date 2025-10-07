using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.MobileApp.PageModels
{
    public partial class LoginPageModel(IAuthService authService) : ObservableObject
    {
        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                await authService.LoginAsync(Email, Password);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Login Failed", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
        }
    }
}
