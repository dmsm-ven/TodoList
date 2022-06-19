using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TodoList.WPF.DataAccess;
using TodoList.WPF.DataAccess.Repositories;
using TodoList.WPF.ViewModels;
using TodoList.WPF.Views;

namespace TodoList.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    readonly IHost host;

    public App()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddAutoMapper(this.GetType().Assembly);

                string connectionString = context.Configuration.GetConnectionString("default");
                services.AddTransient<IDapperDatabaseAccess>(x => new MySqlDapperDatabaseAccess(connectionString));

                ConfigureDatabaseRepositories(services);
                ConfigureViewModels(services);           
            })
            .Build();
    }

    private void ConfigureDatabaseRepositories(IServiceCollection services)
    {
        services.AddTransient<IBookToReadRepository, BookToReadRepository>();
        services.AddTransient<IEmployeerRepository, EmployeerRepository>();
        services.AddTransient<IEmployeerPaymentRepository, EmployeerPaymentRepository>();
        services.AddTransient<IJobItemRepository, JobItemRepository>();
        services.AddTransient<IShoppingItemsRepository, ShoppingItemsRepository>();
    }

    private void ConfigureViewModels(IServiceCollection services)
    {      
        services.AddSingleton<NavigationLocator>();
        services.AddTransient<AddEmployeerWindowViewModel>();
        services.AddTransient<AddEmployeerWindow>();      
        services.AddSingleton<ReadListViewModel>();
        services.AddSingleton<ToolPanelViewModel>();
        services.AddSingleton<AddEmployeerWindowViewModel>();
        services.AddSingleton<ShoppingListViewModel>();
        services.AddSingleton<TodoListViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await host.StartAsync();

        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = host.Services.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await host.StopAsync();
        base.OnExit(e);
    }
}
