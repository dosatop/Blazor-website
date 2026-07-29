using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using UserManagementBlazor.Services;

namespace UserManagementBlazor.Authentication;

public class CustomAuthenticationStateProvider(LocalStorageService storage) : AuthenticationStateProvider
{
    private readonly LocalStorageService _storage = storage;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _storage.GetItemAsync("token");
            var userName = await _storage.GetItemAsync("userName");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, userName ?? "")
            ], "jwt");

            return new AuthenticationState(
                new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyLogin(string userName)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, userName)
        ], "jwt");

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(
                    new ClaimsPrincipal(identity))));
    }

    public void NotifyLogout()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity()))));
    }
}