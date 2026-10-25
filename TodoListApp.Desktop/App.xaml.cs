using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using TodoListApp.ApiClient;
using TodoListApp.Core.Repositories.Interfaces;
using TodoListApp.Desktop.Infrastructure;
using TodoListApp.Desktop.ViewModels.Windows;

namespace TodoListApp.Desktop;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost AppHost { get; private set; }

    public App()
    {
        // По умолчанию WPF разбирает/форматирует числа и даты в привязках по культуре en-US (а не по настройкам Windows),
        // из-за этого "1500,50" в поле цены не принимается. Должно быть выполнено до создания любых окон.
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        // Необработанные исключения в UI-потоке показываем пользователю, а не роняем приложение
        DispatcherUnhandledException += (s, e) =>
        {
            e.Handled = true;
            UiErrors.Show(e.Exception, "Непредвиденная ошибка");
        };

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
                services.AddTransient<RetryGetHandler>();
                services.AddHttpClient(nameof(TodoListAppApiClient), client =>
                {
                    client.BaseAddress = new Uri(context.Configuration["API_HOST"] ?? throw new ArgumentException("API HOST must be provided"));
                    client.DefaultRequestHeaders.Add("X-API-KEY", context.Configuration["API_KEY"] ?? throw new ArgumentException("API KEY must be provided"));
                    client.Timeout = TimeSpan.FromSeconds(20);
                })
                // Клиент живет все время работы приложения (синглтон), поэтому DNS/соединения обновляем через PooledConnectionLifetime
                .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) })
                .AddHttpMessageHandler<RetryGetHandler>();
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
