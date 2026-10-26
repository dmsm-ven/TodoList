using System;
using System.IO;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TodoListApp.Desktop.Infrastructure;

public enum AppTheme
{
    Light,
    Dark
}

/// <summary>Применяет тему WPF-UI "на лету" и запоминает выбор между запусками.</summary>
public class ThemeService
{
    private static readonly string SettingsFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TodoListApp", "theme.txt");

    public AppTheme Current { get; private set; } = AppTheme.Light;

    /// <summary>Читает сохраненную тему и применяет ее (вызывается один раз при старте).</summary>
    public void ApplySaved()
    {
        var theme = AppTheme.Light;
        try
        {
            if (File.Exists(SettingsFile) && Enum.TryParse<AppTheme>(File.ReadAllText(SettingsFile).Trim(), out var saved))
            {
                theme = saved;
            }
        }
        catch
        {
            // Не удалось прочитать файл настроек - остаемся на светлой теме
        }

        Apply(theme, save: false);
    }

    public void Apply(AppTheme theme, bool save = true)
    {
        Current = theme;
        ApplicationThemeManager.Apply(
            theme == AppTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light,
            WindowBackdropType.None,
            true);

        if (!save) { return; }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile)!);
            File.WriteAllText(SettingsFile, theme.ToString());
        }
        catch
        {
            // Тема уже применена; невозможность сохранить выбор не критична
        }
    }
}
