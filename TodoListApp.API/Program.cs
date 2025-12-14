using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.RateLimiting;
using TodoListApp.API.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.ConfigureMyDatabaseRepositories(builder.Configuration);
builder.Services.Configure<ApiKeyConfiguration>(builder.Configuration.GetSection(nameof(ApiKeyConfiguration)));
builder.Services.AddScoped<AuthApiKeyMiddleware>();
builder.Services.AddControllers();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: "global",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60, // максимум 60 запросов
                Window = TimeSpan.FromMinutes(3), // за 3 мин.
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
    options.RejectionStatusCode = 429;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseMiddleware<AuthApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
