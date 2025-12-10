using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using TodoList.WPF.Models.TodoList;
using TodoList.WPF.Services;
using TodoList.WPF.Services.Options;
using TodoList.WPF.ViewModels;
using TodoList.WPF.ViewModels.Windows;
using TodoListApp.Core;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;
using TodoListApp.DataAccess.Repositories.Postgres.Base;
using TodoListApp.DataAccess.Repositories.Postgres.Repositories;

namespace TodoList.WPF;

public static class DependencyInjectionHelper
{
    public static IServiceCollection ConfigureMyDatabaseRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("default");
        services.AddTransient<IDapperDatabaseAccess>(x => new PostgresDapperDatabaseAccess(connectionString));
        services.AddSingleton<IUserDataEncryptValidator, BCryptUserDataValidator>();

        services.AddTransient<IAppLogger, PostgresAppLogger>();
        services.AddTransient<IUserRepository, PostgresBCryptUserValidator>();
        services.AddTransient<IEmployeerRepository, PostgresEmployeerRepository>();
        services.AddTransient<IEmployeerPaymentRepository, PostgresEmployeerPaymentRepository>();
        services.AddTransient<IJobItemRepository, PostgresJobItemRepository>();
        services.AddTransient<ISettingsRepository, PostgresSettingsRepository>();

        return services;
    }
    public static IServiceCollection ConfigureMyServices(this IServiceCollection services)
    {
        services.AddSingleton<UserManager>();

        return services;
    }
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