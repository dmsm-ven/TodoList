using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TodoListApp.WebUI.Models;

namespace TodoListApp.WebUI;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly AppUserLoginConfiguration credentials;
    private readonly string APP_USER_NAME = "app_user";
    private readonly ProtectedLocalStorage protectedLocalStorage;

    public CustomAuthStateProvider(IOptions<AppUserLoginConfiguration> credentialsOptions, ProtectedLocalStorage protectedLocalStorage)
    {
        this.credentials = credentialsOptions.Value;
        this.protectedLocalStorage = protectedLocalStorage;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
    }
    public async Task InitializeAsync()
    {
        try
        {
            var clientSideKey = await protectedLocalStorage.GetAsync<string>("auth_token");
            if (clientSideKey.Success && clientSideKey.Value.Equals(credentials.Token))
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, APP_USER_NAME) }, "token_auth");
                var userPrincipal = new ClaimsPrincipal(identity);
                var loggedState = Task.FromResult(new AuthenticationState(userPrincipal));
                NotifyAuthenticationStateChanged(loggedState);
            }
        }
        catch (Exception ex)
        {

        }
    }
}