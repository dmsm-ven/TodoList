using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace TodoListApp.Web;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IConfiguration configuration;
    private readonly ProtectedLocalStorage storage;
    public static readonly string LocalStorageKeyName = "auth_token";
    private Lazy<string> appToken => new Lazy<string>(() => configuration["API_KEY"] ?? throw new InvalidOperationException("AuthToken is not configured"));

    public CustomAuthStateProvider(IConfiguration configuration, ProtectedLocalStorage storage)
    {
        this.configuration = configuration;
        this.storage = storage;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userToken = await storage.GetAsync<string>(LocalStorageKeyName);
            if (userToken.Success && userToken.Value == appToken.Value)
            {
                return AuthorizedState();
            }
        }
        catch
        {

        }

        return GetDefaultState();
    }

    public AuthenticationState AuthorizedState()
    {   
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "app_user"),
            new Claim(ClaimTypes.Role, "app_user")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var state = new AuthenticationState(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;
    }

    private AuthenticationState GetDefaultState()
    {
        storage.DeleteAsync(LocalStorageKeyName);

        var identity = new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.Name, "anonym")
            }, authenticationType: string.Empty);

        var anonym = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonym)));
        var state = new AuthenticationState(anonym);
        return state;
    }
}
