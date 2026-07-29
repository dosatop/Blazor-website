using UserManagementBlazor.Components;
using UserManagementBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using UserManagementBlazor.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://usermanagementapi-k9f6.onrender.com/")
});
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider,
    CustomAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider,
    CustomAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();

// builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();

// app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();