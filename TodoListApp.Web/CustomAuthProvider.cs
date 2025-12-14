using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace TodoListApp.Web;

public class CustomAuthProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "CustomUser"),
            new Claim(ClaimTypes.Role, "app_user"),
        };
        var identity = new ClaimsIdentity(claims, "CustomAuthType");
        var user = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(user));
    }
}
