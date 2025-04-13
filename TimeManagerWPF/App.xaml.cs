using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using TodoList.WPF.Services;
using TodoList.WPF.ViewModels.Windows;
using TodoList.WPF.Views;

namespace TodoList.WPF;
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
                services.ConfigureMyOptions();
                services.ConfigureMyDatabaseRepositories(context.Configuration);
                services.ConfigureMyServices();
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

    protected override void OnExit(ExitEventArgs e)
    {
        AppHost.Services.GetRequiredService<UserManager>()?.ApplicationClosed();
    }
}
