namespace Market.MobileApp
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;
        public App(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var isAuthenticated = _authService.IsUserAuthenticatedAsync().Result;

            Page page = isAuthenticated ? new AppShell() : new LoginPage(new LoginPageModel(_authService));

            return new Window(page);
        }
    }
}