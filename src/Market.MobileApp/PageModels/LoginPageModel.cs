using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.MobileApp.PageModels
{
    public partial class LoginPageModel : ObservableObject
    {
        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [RelayCommand]
        private async Task LoginAsync()
        {
            // Implement login logic here
            await Task.Delay(1000); // Simulate a login delay
        }

        public LoginPageModel()
        {
        }

        [RelayCommand]
        private async Task Register()
        {
            App.Current.MainPage = new RegisterPage();
        }
    }
}
