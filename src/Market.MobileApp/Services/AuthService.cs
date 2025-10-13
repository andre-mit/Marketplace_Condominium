using Market.SharedApplication.ViewModels.AuthViewModels;
using Market.SharedApplication.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Market.MobileApp.Services
{
    public class AuthService(HttpClient httpClient) : IAuthService
    {
        private const string AuthPath = "/auth";
        private const string UsersPath = "/users";
        public async Task ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var request = new ChangePasswordRequestViewModel(currentPassword, newPassword);
                var response = await httpClient.PostAsJsonAsync($"{AuthPath}/change-password", request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
            }
            catch (Exception)
            {
            }
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            return false;
        }

        public async Task<string?> GetAuthTokenAsync()
        {
            return await SecureStorage.GetAsync("access_token");
        }

        public async Task<bool> IsUserAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync("access_token");
            return !string.IsNullOrEmpty(token);
        }

        public async Task LoginAsync(string email, string password)
        {
            try
            {
                var request = new LoginRequestViewModel(email, password);
                var response = await httpClient.PostAsJsonAsync($"{AuthPath}/login", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();
                if (result is not null)
                {
                    // Store the token securely, e.g., in secure storage
                    var token = result.AccessToken;
                    var user = result.User;

                    await SecureStorage.Default.SetAsync("access_token", token);

                    Preferences.Default.Set("user_email", user.Email);
                    Preferences.Default.Set("user_first_name", user.FirstName);
                    Preferences.Default.Set("user_last_name", user.LastName);
                    Preferences.Default.Set("user_id", user.Id.ToString());
                    Preferences.Default.Set("user_tower", user.Tower);
                    Preferences.Default.Set("user_unit", user.Unit);

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (HttpRequestException)
            {
                // TODO: Handle HTTP request exceptions
            }
            catch (Exception)
            {
                // TODO: Handle other exceptions
            }
        }

        public void Logout()
        {
            SecureStorage.Default.RemoveAll();
            Preferences.Default.Remove("user_email");
            Preferences.Default.Remove("user_first_name");
            Preferences.Default.Remove("user_last_name");
            Preferences.Default.Remove("user_id");
            Preferences.Default.Remove("user_tower");
            Preferences.Default.Remove("user_unit");

            httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task RegisterAsync(CreateUserViewModel user)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync($"{UsersPath}", user);
                response.EnsureSuccessStatusCode();

                var registeredUser = await response.Content.ReadFromJsonAsync<ListUserViewModel>();
                if(registeredUser is not null)
                {
                    await LoginAsync(user.Email, user.Password);
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (Exception)
            {
            }
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var response = await httpClient.PostAsync($"{AuthPath}/password-reset?email={Uri.EscapeDataString(email)}", null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
            }
            catch (Exception)
            {
            }
        }
    }
}
