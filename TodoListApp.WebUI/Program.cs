using System;
using TodoListApp.ApiClient;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.WebUI.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient(nameof(TodoListAppApiClient), client =>
{
    client.BaseAddress = new Uri(builder.Configuration["API_HOST"] ?? throw new ArgumentException("API HOST must be provided"));
    client.DefaultRequestHeaders.Add("X-API-KEY", builder.Configuration["API_KEY"] ?? throw new ArgumentException("API KEY must be provided"));
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
