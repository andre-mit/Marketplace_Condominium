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
        public PasswordEntryModel(string placeholder, string password, bool isPassword)
        {
            Placeholder = placeholder;
            Text = password;
            IsPassword = isPassword;
        }

        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private string _placeholder;

        [ObservableProperty]
        private bool _isPassword;


        [ObservableProperty]
        private ImageSource _icon;

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPassword = !IsPassword;

            var iconName = IsPassword ? "IconEyeHide" : "IconEye";

            Application.Current?.Resources.MergedDictionaries.Where(md => md.Source.OriginalString.Contains("AppStyles")).ToList().ForEach(dictionary =>
            {
                if (dictionary.TryGetValue(iconName, out var resource))
                {
                    Icon = resource as ImageSource;
                }
            });
        }
    }
}
