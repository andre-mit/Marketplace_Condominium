using Market.MobileApp.PageModels.ControlModels;
using System.Linq;

namespace Market.MobileApp.Pages.Controls;

public partial class PasswordEntry : ContentView
{
    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(
            propertyName: nameof(IsPassword),
            returnType: typeof(bool),
            declaringType: typeof(PasswordEntry),
            defaultValue: true);

    private bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(PasswordEntry),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);


    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(
            propertyName: nameof(Placeholder),
            returnType: typeof(string),
            declaringType: typeof(PasswordEntry),
            defaultValue: null);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public PasswordEntry()
    {
        InitializeComponent();
        BindingContext = new PasswordEntryModel(Placeholder, Text, IsPassword);
    }

    private void OnToggleVisibilityClicked(object sender, EventArgs e)
    {
        IsPassword = !IsPassword;

        var iconName = IsPassword ? "IconEyeHide" : "IconEye";

        Application.Current?.Resources.MergedDictionaries.Where(md => md.Source.OriginalString.Contains("AppStyles")).ToList().ForEach(dictionary =>
        {
            if (dictionary.TryGetValue(iconName, out var resource))
            {
                var button = (Button)sender;
                button.ImageSource = resource as ImageSource;
            }
        });
    }
}