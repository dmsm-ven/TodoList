using Microsoft.Extensions.Options;
using PainvenNotificator;
using System.Net;
using TodoListApp.API.Model;
using TodoListApp.Core.Repositories.Interfaces;

internal class AuthApiKeyMiddleware : IMiddleware
{
    public static readonly string API_KEY_HEADER_NAME = "X-API-KEY";
    private readonly string apiKey;
    private readonly ILogger<AuthApiKeyMiddleware> logger;
    private readonly IAppLogger appLogger;
    private readonly IApiEventNotificator notifier;

    public AuthApiKeyMiddleware(IOptions<ApiKeyConfiguration> options,
        ILogger<AuthApiKeyMiddleware> logger,
        IAppLogger appLogger,
        IApiEventNotificator notifier)
    {
        this.apiKey = options.Value.ApiKey;
        this.logger = logger;
        this.appLogger = appLogger;
        this.notifier = notifier;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string clientIp = context.Request.Headers.ContainsKey("X-Real-IP") ? context.Request.Headers["X-Real-IP"].ToString() : "";

        if (!IPAddress.TryParse(clientIp, out _))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("400 Bad Request");
            return;
        }

        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("400 Bad Request");
            logger.LogWarning("Attempt access to api without API KEY c IP {clientIp}", clientIp);

            _ = notifier.Notify("Попытка доступа к API без API KEY", clientIp);

            return;
        }

        if (apiKey != extractedApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("401 Unauthorized");
            logger.LogWarning("Attempt access to api with invalid API KEY c IP {clientIp}", clientIp);

            _ = notifier.Notify("Попытка доступа к API с неверным API KEY", clientIp);

            return;
        }

        logger.LogInformation("[{clientIp}] Обращение к API {patch}", clientIp, context.Request.Path);
        _ = appLogger.WriteLog(clientIp, context.Request.Path, "Обращение к API");

        await next(context);
    }
}