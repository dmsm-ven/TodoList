using Microsoft.AspNetCore.Components.Authorization;
using TodoListApp.ApiClient;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.WebUI;
using TodoListApp.WebUI.Components;
using TodoListApp.WebUI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AppUserLoginConfiguration>(builder.Configuration.GetSection(nameof(AppUserLoginConfiguration)));
builder.Services.AddHttpClient(nameof(TodoListAppApiClient), client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiConfiguration:Host"] ?? throw new ArgumentException("API HOST must be provided"));
    client.DefaultRequestHeaders.Add("X-API-KEY", builder.Configuration["ApiConfiguration:Token"] ?? throw new ArgumentException("API KEY must be provided"));
});
builder.Services.AddSingleton<TodoListAppApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient(nameof(TodoListAppApiClient));
    return new TodoListAppApiClient(httpClient);
});
builder.Services.AddSingleton<IAppLogger>(x => x.GetRequiredService<TodoListAppApiClient>());
builder.Services.AddSingleton<IJobItemRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
builder.Services.AddSingleton<IEmployeerRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthentication();
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

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
