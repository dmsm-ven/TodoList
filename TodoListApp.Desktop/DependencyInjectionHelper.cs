using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using TodoListApp.Desktop.Models.TodoList;
using TodoListApp.Desktop.Services;
using TodoListApp.Desktop.Services.Options;
using TodoListApp.Desktop.ViewModels;
using TodoListApp.Desktop.ViewModels.Windows;
using TodoListApp.Core;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Desktop;

public static class DependencyInjectionHelper
{
    public static IServiceCollection ConfigureMyOptions(this IServiceCollection services)
    {
        services.Configure<ScreenshotFolderOptions>(x =>
        new ScreenshotFolderOptions(Path.Combine(Directory.GetCurrentDirectory(), "screenshots")));

        return services;
    }
    public static IServiceCollection ConfigureFactoryInitializators(this IServiceCollection services)
    {
        services.AddSingleton<Func<EmployeerEntity, EmployeerTabViewModel>>(
            (x) => new Func<EmployeerEntity, EmployeerTabViewModel>(
                (empEntity) =>
                {
                    var tabVm = App.AppHost.Services.GetService<EmployeerTabViewModel>();
                    tabVm.SetEmployeer(empEntity.ToViewModel());
                    return tabVm;
                }));

        return services;
    }
    public static IServiceCollection ConfigureMyViewModels(this IServiceCollection services)
    {
        services.AddTransient<EmployeerTabViewModel>();
        services.AddSingleton<ConnectionErrorWindowViewModel>();
        services.AddSingleton<LoginWindowViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<AddEmployeerWindowViewModel>();
        services.AddSingleton<TodoListViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }
}