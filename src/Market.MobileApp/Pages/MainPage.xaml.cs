using Market.MobileApp.Models;
using Market.MobileApp.PageModels;

namespace Market.MobileApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}