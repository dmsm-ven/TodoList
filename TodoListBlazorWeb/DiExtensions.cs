using TodoListApp.Core;
using TodoListApp.DataAccess.Repositories.Interfaces;
using TodoListApp.DataAccess.Repositories.Postgres.Base;
using TodoListApp.DataAccess.Repositories.Postgres.Repositories;

public static class DiExtensions
{
    public static IServiceCollection ConfigureMyDatabaseRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("default");
        services.AddTransient<IDapperDatabaseAccess>(x => new PostgresDapperDatabaseAccess(connectionString));
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