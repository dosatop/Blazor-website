using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using UserManagementBlazor.Authentication;
using UserManagementBlazor.Dtos;

namespace UserManagementBlazor.Services
{
    public class AuthService(HttpClient http, LocalStorageService localStorage, CustomAuthenticationStateProvider authStateProvider)
    {
        private readonly HttpClient _http = http;
        private readonly LocalStorageService _localStorage = localStorage;
        private readonly CustomAuthenticationStateProvider _authStateProvider = authStateProvider;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync("token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task EnsureTokenAsync()
        {
            if (_http.DefaultRequestHeaders.Authorization != null)
                return;

            var token = await _localStorage.GetItemAsync("token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            await EnsureTokenAsync();

            return await _http.GetFromJsonAsync<List<UserDto>>("api/users")
                   ?? [];
        }
        public async Task<LoginResponse?> Login(LoginRequest request)
        {

            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            var responseText = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"API Response: {responseText}");

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content
                    .ReadFromJsonAsync<ApiErrorResponse>();

                var message = errorResponse?.Errors.FirstOrDefault()
                              ?? "Login failed";

                throw new Exception(message);
            }

            var loginResponse =
                JsonSerializer.Deserialize<LoginResponse>(
                    responseText,
                    _jsonOptions);

            if (loginResponse != null)
            {
                await _localStorage.SetItemAsync(
                    "token",
                    loginResponse.AccessToken);

                _authStateProvider.NotifyLogin(request.Email);
            }

            return loginResponse;
        }
    }
}