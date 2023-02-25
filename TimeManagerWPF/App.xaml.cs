using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
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
            //.ConfigureAppConfiguration(options =>
            //{
            //    options.AddUserSecrets(assembly, optional: true);
            //})
            .ConfigureServices((context, services) =>
            {
                string connectionString = context.Configuration.GetConnectionString("default");
                services.AddTransient<IDapperDatabaseAccess>(x => new PostgresDapperDatabaseAccess(connectionString));

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
            Dictionary<string, string> eArgs = ParseEventArgs(e.Args);

            ShowLoginWindow(eArgs);
        }
        catch (Exception ex)
        {
            ShowErrorWindow(ex.Message);
        }
    }

    private static Dictionary<string, string> ParseEventArgs(string[] args)
    {
        var dic = new Dictionary<string, string>();
        foreach (var item in args)
        {
            if (item.StartsWith("--") && item.Contains("="))
            {
                string key = item.Substring(2, item.IndexOf('=') - 2);
                string value = item.Substring(item.IndexOf("=") + 1);
                dic[key] = value;
                //MessageBox.Show($"key={key};value={value}");
            }
        }

        return dic;
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
    }

    private void ConfigureViewModels(IServiceCollection services)
    {
        services.AddSingleton<ConnectionErrorWindow>();
        services.AddSingleton<ConnectionErrorWindowViewModel>();

        services.AddSingleton<LoginWindow>();
        services.AddSingleton<LoginWindowViewModel>();

        services.AddSingleton<NavigationLocator>();

        services.AddSingleton<BudgetViewModel>();
        services.AddSingleton<BudgetView>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<ConnectionErrorWindowViewModel>();
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

    private void ShowErrorWindow(string errorMessage)
    {
        var errorWindow = host.Services.GetRequiredService<ConnectionErrorWindow>();
        var vm = host.Services.GetRequiredService<ConnectionErrorWindowViewModel>();
        vm.ErrorMessage = errorMessage;
        errorWindow.DataContext = vm;
        errorWindow.Show();
    }

    private void ShowLoginWindow(IReadOnlyDictionary<string, string> e)
    {
        var loginWindowVm = host.Services.GetRequiredService<LoginWindowViewModel>();
        if (e.TryGetValue("login", out var login))
        {
            loginWindowVm.Login = login;
        }
        if (e.TryGetValue("pass", out var pass))
        {
            loginWindowVm.DefaultPassword = pass;
        }

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

        // 3/4 ширины-высоты
        mainWindow.Left = SystemParameters.PrimaryScreenWidth / 4;
        mainWindow.Top = SystemParameters.PrimaryScreenHeight / 4;
        mainWindow.Width = SystemParameters.PrimaryScreenWidth * 0.75d;
        mainWindow.Height = SystemParameters.PrimaryScreenHeight * 0.75d;

        mainWindow.DataContext = host.Services.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }

    private void ApplicationAlreadyRunningCheck()
    {
        Process proc = Process.GetCurrentProcess();
        int count = Process.GetProcesses().Where(p => p.ProcessName == proc.ProcessName).Count();

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
