
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using TodoListApp.API.Model;
using TodoListApp.Core.Repositories.Interfaces;

internal class AuthApiKeyMiddleware : IMiddleware
{
    public static readonly string API_KEY_HEADER_NAME = "X-API-KEY";
    private readonly string apiKey;
    private readonly ILogger<AuthApiKeyMiddleware> logger;
    private readonly IAppLogger appLogger;

    public AuthApiKeyMiddleware(IOptions<ApiKeyConfiguration> options, 
        ILogger<AuthApiKeyMiddleware> logger,
        IAppLogger appLogger)
    {
        this.apiKey = options.Value.ApiKey;
        this.logger = logger;
        this.appLogger = appLogger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string clientIp = context.Request.Headers.ContainsKey("X-Real-IP") ? context.Request.Headers["X-Real-IP"].ToString() : "";

        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("400 Bad Request");
            logger.LogWarning("Attempt access to api without API KEY c IP {clientIp}", clientIp);
            return;
        }

        if(apiKey != extractedApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("401 Unauthorized");
            logger.LogWarning("Attempt access to api with invalid API KEY c IP {clientIp}", clientIp);
            return;
        }

        logger.LogInformation("Пользователь обратился к API по пути {path} с IP {clientIp}", context.Request.Path, clientIp);
        appLogger.WriteLog($"Пользователь обратился к API по пути {context.Request.Path} с IP {clientIp}");

        await next(context);
    }
}