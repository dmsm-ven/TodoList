using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using TodoListApp.Desktop.Infrastructure;

namespace TodoListApp.Desktop.ViewModels;

public record ThemeOption(AppTheme Theme, string Title);

public partial class AppSettingsViewModel : ObservableObject
{
    private readonly ThemeService themeService;

    public IReadOnlyList<ThemeOption> Themes { get; } = new[]
    {
        new ThemeOption(AppTheme.Light, "Светлая"),
        new ThemeOption(AppTheme.Dark, "Тёмная"),
    };

    [ObservableProperty]
    private ThemeOption selectedTheme;

    public AppSettingsViewModel(ThemeService themeService)
    {
        this.themeService = themeService;
        selectedTheme = Themes.First(t => t.Theme == themeService.Current);
    }

    partial void OnSelectedThemeChanged(ThemeOption value)
    {
        if (value != null)
        {
            themeService.Apply(value.Theme);
        }
    }
}
