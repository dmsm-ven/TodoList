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
using Microsoft.Extensions.Configuration;

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
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets(assembly, optional: true);
            })
            .UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient(nameof(TodoListAppApiClient), client =>
                {
                    client.BaseAddress = new Uri(context.Configuration["API_HOST"] ?? throw new ArgumentException("API HOST must be provided"));
                    client.DefaultRequestHeaders.Add("X-API-KEY", context.Configuration["API_KEY"] ?? throw new ArgumentException("API KEY must be provided"));
                });
                services.AddSingleton<TodoListAppApiClient>(sp =>
                {
                    var factory = sp.GetRequiredService<IHttpClientFactory>();
                    var httpClient = factory.CreateClient(nameof(TodoListAppApiClient));
                    return new TodoListAppApiClient(httpClient);
                });
                services.AddSingleton<IAppLogger>(x => x.GetRequiredService<TodoListAppApiClient>());
                services.AddSingleton<IJobItemRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
                services.AddSingleton<IEmployeerRepository>(x => x.GetRequiredService<TodoListAppApiClient>());
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
