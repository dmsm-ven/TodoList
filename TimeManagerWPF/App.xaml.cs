using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using TodoList.WPF.DataAccess;
using TodoList.WPF.DataAccess.Repositories;
using TodoList.WPF.Models;
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
        Assembly assembly = this.GetType().Assembly;

        Directory.SetCurrentDirectory(Path.GetDirectoryName(assembly.Location));

        host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(options =>
            {
                options.AddUserSecrets(assembly, optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                string connectionString = context.Configuration.GetConnectionString("default");              
                services.AddTransient<IDapperDatabaseAccess>(x => new MySqlDapperDatabaseAccess(connectionString));

                ConfigureDatabaseRepositories(services);
                ConfigureServices(services);
                ConfigureViewModels(services);
                services.AddAutoMapper(assembly);
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Проверяем, запущена ли уже копия приложения, если да - закрываем эту копию
        ApplicationAlreadyRunningCheck();

        await host.StartAsync();
       
        try
        {
            bool alreadyLoginToday = host.Services.GetRequiredService<UserManager>().TryLoginWithSavedPassword();
            
            if (alreadyLoginToday)
            {
                ShowMainWindow();
            }
            else
            {
                ShowLoginWindow();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Нет соединения с базой данных.\r\n" +  ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }

    private void ConfigureDatabaseRepositories(IServiceCollection services)
    {
        services.AddTransient<IAppLogger, AppLogger>();
        services.AddTransient<IUserRepository, BCryptUserValidator>();
        services.AddTransient<IBookToReadRepository, BookToReadRepository>();
        services.AddTransient<IEmployeerRepository, EmployeerRepository>();
        services.AddTransient<IEmployeerPaymentRepository, EmployeerPaymentRepository>();
        services.AddTransient<IJobItemRepository, JobItemRepository>();
        services.AddTransient<IShoppingItemsRepository, ShoppingItemsRepository>();
        services.AddTransient<ISettingsRepository, SettingsRepository>();
    }

    private void ConfigureViewModels(IServiceCollection services)
    {
        services.AddSingleton<LoginWindow>();
        services.AddSingleton<LoginWindowViewModel>();

        services.AddSingleton<NavigationLocator>();

        services.AddSingleton<SettingsViewModel>();        
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

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<UserManager>();
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
        try
        {
            if (Application.Current.MainWindow != null)
            {
                host.Services.GetRequiredService<UserManager>().ApplicationClosed();               
            }
            await host.StopAsync();
        }
        catch
        {

        }
    }
}
