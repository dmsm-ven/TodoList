using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using TodoListApp.ApiClient;
using Microsoft.Extensions.Http;
using TodoListApp.Desktop.Services;
using TodoListApp.Desktop.ViewModels.Windows;
using TodoListApp.Desktop.Views;
using TodoListApp.Core.Repositories.Interfaces;
using System;
using System.Net.Http;

namespace TodoListApp.Desktop;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost AppHost { get; private set; }

    public App()
    {
        Assembly assembly = this.GetType().Assembly;

        Directory.SetCurrentDirectory(Path.GetDirectoryName(assembly.Location));

        AppHost = Host.CreateDefaultBuilder()
            .UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient(nameof(TodoListAppApiClient), client =>
                {
                    client.BaseAddress = new Uri(context.Configuration["ApiHost"]!);
                });
                services.AddSingleton<TodoListAppApiClient>(sp =>
                {
                    var factory = sp.GetRequiredService<IHttpClientFactory>();
                    var httpClient = factory.CreateClient(nameof(TodoListAppApiClient));
                    return new TodoListAppApiClient(httpClient);
                });
                services.AddSingleton<IUserRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
                services.AddSingleton<ISettingsRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
                services.AddSingleton<IJobItemRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
                services.AddSingleton<IEmployeerRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
                services.AddSingleton<IEmployeerPaymentRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
                services.ConfigureMyOptions();
                services.ConfigureFactoryInitializators();
                services.ConfigureMyViewModels();

            })
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        ShowMainWindow();
    }

    private void ShowLoginWindow()
    {
        var loginWindowViewModel = AppHost.Services.GetRequiredService<LoginWindow>();
        LoginWindow loginWindow = new();
        loginWindow.DataContext = loginWindowViewModel;
        loginWindow.Show();
    }

    private void ShowMainWindow()
    {
        var mainWindow = new MainWindow();

        // 3/4 ширины-высоты
        mainWindow.Left = SystemParameters.PrimaryScreenWidth / 4;
        mainWindow.Top = SystemParameters.PrimaryScreenHeight / 4;
        mainWindow.Width = SystemParameters.PrimaryScreenWidth * 0.75d;
        mainWindow.Height = SystemParameters.PrimaryScreenHeight * 0.75d;

        mainWindow.DataContext = AppHost.Services.GetRequiredService<MainWindowViewModel>();
        mainWindow.ShowDialog();
    }
}
