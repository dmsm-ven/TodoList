using TodoListApp.Core;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.Core.Repositories.Postgres.Base;
using TodoListApp.Core.Repositories.Postgres.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureMyDatabaseRepositories(builder.Configuration);
builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public static class DependencyInjectionHelper
{
    public static IServiceCollection ConfigureMyDatabaseRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDapperDatabaseAccess>(x => new PostgresDapperDatabaseAccess(configuration.GetConnectionString("default")!));
        services.AddSingleton<IUserDataEncryptValidator, BCryptUserDataValidator>();
        services.AddTransient<IAppLogger, PostgresAppLogger>();
        services.AddTransient<IUserRepository, PostgresBCryptUserValidator>();
        services.AddTransient<IEmployeerRepository, PostgresEmployeerRepository>();
        services.AddTransient<IEmployeerPaymentRepository, PostgresEmployeerPaymentRepository>();
        services.AddTransient<IJobItemRepository, PostgresJobItemRepository>();
        services.AddTransient<ISettingsRepository, PostgresSettingsRepository>();

        return services;
    }
}