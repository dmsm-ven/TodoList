using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using TodoListApp.ApiClient;
using TodoListApp.Core;
using TodoListApp.Core.Repositories.Interfaces;

public static class DiExtensions
{
    public static IServiceCollection ConfigureMyServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(nameof(TodoListAppApiClient), client =>
        {
            client.BaseAddress = new Uri(configuration["API_HOST"] ?? throw new ArgumentException("API HOST must be provided"));
            client.DefaultRequestHeaders.Add("X-API-KEY", configuration["API_KEY"] ?? throw new ArgumentException("API KEY must be provided"));
        });

        services.AddSingleton<TodoListAppApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = factory.CreateClient(nameof(TodoListAppApiClient));
            return new TodoListAppApiClient(httpClient);
        });
        services.AddSingleton<IAppLogger>(x => x.GetRequiredService<TodoListAppApiClient>());
        services.AddSingleton<IJobItemRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
        services.AddSingleton<IEmployeerRepository>(x => x.GetRequiredService<TodoListAppApiClient>());

        return services;
    }
}