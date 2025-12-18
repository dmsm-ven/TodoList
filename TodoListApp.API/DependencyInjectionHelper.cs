using TodoListApp.API.DataAccess;
using TodoListApp.Core;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.Core.Repositories.Postgres.Base;
using TodoListApp.Core.Repositories.Postgres.Repositories;

public static class DependencyInjectionHelper
{
    public static IServiceCollection ConfigureMyDatabaseRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDapperDatabaseAccess>(x => new PostgresDapperDatabaseAccess(configuration.GetConnectionString("default")!));
        services.AddSingleton<IUserDataEncryptValidator, BCryptUserDataValidator>();
        services.AddTransient<IAppLogger, PostgresAppLogger>();
        services.AddTransient<IUserRepository, PostgresBCryptUserValidator>();
        services.AddTransient<IEmployeerRepository, PostgresEmployeerRepository>();
        services.AddScoped<IJobItemRepository, PostgresJobItemRepository>();
        services.Decorate<IJobItemRepository, CachedJobItemRepository>();

        return services;
    }
}