using TodoListApp.WebUI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.ResolveAppDependencies();
builder.Services.AddAuthentication("Custom").AddCookie("Custom", options =>
{
    options.LoginPath = "/todoweb/login";
    options.LogoutPath = "/todoweb/login";
    options.AccessDeniedPath = "/todoweb";
    options.Cookie.Path = "/todoweb";

    // Prevent redirect loops for Blazor Server
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = 406;
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UseAntiforgery();
app.UsePathBase("/todoweb");
app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


