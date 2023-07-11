using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoList.Avalonia.ViewModels;
using TodoList.Avalonia.Views;
using TodoListApp.Core;
using TodoListApp.DataAccess.Repositories.Interfaces;
using TodoListApp.DataAccess.Repositories.Postgres.Base;
using TodoListApp.DataAccess.Repositories.Postgres.Repositories;

namespace TodoList.Avalonia;

public partial class App : Application
{
    private IHost host = null;
    public override void Initialize()
    {
        ResolveDependencies();
        AvaloniaXamlLoader.Load(this);
    }

    private void ResolveDependencies()
    {
        host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddTransient<IDapperDatabaseAccess>(x =>
            {
                string connectionString = context.Configuration.GetConnectionString("default");
                return new PostgresDapperDatabaseAccess(connectionString);
            });
            services.AddSingleton<IUserDataEncryptValidator, BCryptUserDataValidator>();

            ConfigureDatabaseRepositories(services);

            services.AddSingleton<MainViewModel>();

        })
        .Build();
    }

    private void ConfigureDatabaseRepositories(IServiceCollection services)
    {
        services.AddTransient<IAppLogger, PostgresAppLogger>();
        services.AddTransient<IUserRepository, PostgresBCryptUserValidator>();
        services.AddTransient<IBookToReadRepository, PostgresBookToReadRepository>();
        services.AddTransient<IEmployeerRepository, PostgresEmployeerRepository>();
        services.AddTransient<IEmployeerPaymentRepository, PostgresEmployeerPaymentRepository>();
        services.AddTransient<IJobItemRepository, PostgresJobItemRepository>();
        services.AddTransient<IShoppingItemsRepository, PostgresShoppingItemsRepository>();
        services.AddTransient<ISettingsRepository, PostgresSettingsRepository>();
        services.AddTransient<IBudgetRepository, PostgresBudgetRepository>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = host.Services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = host.Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
