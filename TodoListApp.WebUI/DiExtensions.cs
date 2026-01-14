using TodoListApp.ApiClient;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.WebUI.Models;

public static class AuthDefaults
{
    public const string AuthScheme = "login_page_cookie_auth";
}

public static class DiExtensions
{

    public static WebApplicationBuilder ResolveAppDependencies(this WebApplicationBuilder builder)
    {
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



        return builder;
    }

    public static WebApplicationBuilder AddNotificatorSender(this WebApplicationBuilder builder)
    {
        //builder.Services.AddHttpClient();
        //builder.Services.Configure<GeoDataExtractorConfiguration>(builder.Configuration.GetSection(nameof(GeoDataExtractorConfiguration)));
        //builder.Services.Configure<GeoDataExtractorConfiguration>(builder.Configuration.GetSection(nameof(TelegramConfiguration)));
        //builder.Services.AddSingleton<IGeoDataExtractor, BasicGeoDataExtractor>();
        //builder.Services.AddSingleton<IApiEventNotificator, TelegramApiEventNotificator>();

        return builder;
    }

}