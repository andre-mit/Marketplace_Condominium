using CommunityToolkit.Mvvm.ComponentModel;

namespace Market.MobileApp.Pages.Controls;

public partial class PasswordEntry : ContentView
{
    private bool isPasswordVisible = false;

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
    }

    private void OnToggleVisibilityClicked(object sender, EventArgs e)
    {
        //isPasswordVisible = !isPasswordVisible;
        //PasswordInput.IsPassword = !isPasswordVisible;

        //// Atualiza a imagem do botão
        //var button = (ImageButton)sender;
        //button.Source = isPasswordVisible
        //    ? "eye_hidden_icon.png"
        //    : "eye_visible_icon.png";
    }
}