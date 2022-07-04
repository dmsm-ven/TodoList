using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        try
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(this.GetType().Assembly.Location));

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
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void ConfigureDatabaseRepositories(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, BCryptUserValidator>();
        services.AddTransient<IBookToReadRepository, BookToReadRepository>();
        services.AddTransient<IEmployeerRepository, EmployeerRepository>();
        services.AddTransient<IEmployeerPaymentRepository, EmployeerPaymentRepository>();
        services.AddTransient<IJobItemRepository, JobItemRepository>();
        services.AddTransient<IShoppingItemsRepository, ShoppingItemsRepository>();
    }

    private void ConfigureViewModels(IServiceCollection services)
    {
        services.AddSingleton<LoginWindow>();
        services.AddSingleton<LoginWindowViewModel>();

        services.AddSingleton<NavigationLocator>();
        services.AddTransient<ConnectionErrorViewModel>();
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
        try
        {
            ApplicationAlreadyRunningCheck();

            await host.StartAsync();

            var userRepository = host.Services.GetRequiredService<IUserRepository>();

            if (userRepository.TryLoginWithSavedPassword()) // ок - зашли без пароля, т.к. сегодня уже пароль был успешно введен
            {
                ShowMainWindow();
            }
            else
            {
                ShowLoginWindow();
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message + "; cs = " + host.Services.GetRequiredService<IConfiguration>().GetConnectionString("default"));
        }
    }

    private void ShowLoginWindow()
    {
        var loginWindowVm = host.Services.GetRequiredService<LoginWindowViewModel>();
        var loginWindow = host.Services.GetRequiredService<LoginWindow>();
        loginWindow.DataContext = loginWindowVm;
        loginWindowVm.OnUserEnter += () =>
        {
            ShowMainWindow();
            loginWindow.Close();
        };
        loginWindow.Show();
    }

    private void ShowMainWindow()
    {
        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = host.Services.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }

    private void ApplicationAlreadyRunningCheck()
    {
        Process proc = Process.GetCurrentProcess();
        int count = Process.GetProcesses().Where(p =>p.ProcessName == proc.ProcessName).Count();
        
        if (count > 1)
        {
            App.Current.Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await host.StopAsync();
        base.OnExit(e);
    }
}
