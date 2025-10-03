using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.MobileApp.PageModels.ControlModels
{
    public partial class PasswordEntryModel : ObservableObject
    {
        public PasswordEntryModel() { }

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isPasswordVisible = false;

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}
