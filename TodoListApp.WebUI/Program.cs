using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PainvenNotificator;
using System.Security.Claims;
using TodoListApp.WebUI.Components;
using TodoListApp.WebUI.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAntiforgery(options =>
{
    //options.Cookie.Path = "/todoweb";
    options.Cookie.Path = "/";
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(AuthDefaults.AuthScheme).AddCookie(AuthDefaults.AuthScheme, options =>
{
    //options.LoginPath = "/todoweb/login";
    //options.AccessDeniedPath = "/todoweb";
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/";
});
builder.ResolveAppDependencies();
builder.AddNotificatorSender();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
app.MapPost("/login-check", async ([FromForm] LoginModel loginModel,
    IOptions<AppUserLoginConfiguration> options,
    IHttpContextAccessor httpCa) =>
{
    string validToken = options.Value.Token;

    await Task.Delay(Random.Shared.Next(500, 1000));

    if (!loginModel.UserToken.Equals(validToken))
    {
        return Results.BadRequest();
    }

    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "app_user")
            };

    var identity = new ClaimsIdentity(claims, AuthDefaults.AuthScheme);
    var principal = new ClaimsPrincipal(identity);

    await httpCa.HttpContext!.SignInAsync(
        AuthDefaults.AuthScheme,
        principal,
        new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddDays(31)
        });

    return Results.Redirect("/todoweb/tasks");
}).WithMetadata(new IgnoreAntiforgeryTokenAttribute());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var appNotificator = app.Services.GetRequiredService<IApiEventNotificator>();
app.Lifetime.ApplicationStopping.Register(() =>
{
    appNotificator.Notify("TODO Web приложение остановлено");
});
app.Lifetime.ApplicationStarted.Register(() =>
{
    appNotificator.Notify("TODO Web приложение запущено");
});

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UsePathBase("/todoweb");
app.UseStaticFiles();
app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


