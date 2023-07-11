using System.Collections.Generic;
using System.Windows.Input;
using TodoList.WPF.Services;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;

namespace TodoList.WPF.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private bool isLoaded = false;
    private readonly ISettingsRepository seetingsRepository;
    private readonly MainWindowViewModel mainWindowViewModel;
    private readonly UserManager userManager;
    private bool isTopmost = false;
    public bool IsTopmost
    {
        get => isTopmost;
        set
        {
            if (Set(ref isTopmost, value) && isLoaded)
            {
                seetingsRepository.Set(nameof(IsTopmost), value.ToString());
                mainWindowViewModel.IsTopmost = value;
            }
        }
    }

    private IEnumerable<LogEntryEntity> logEnties;
    public IEnumerable<LogEntryEntity> LogEntries
    {
        get => logEnties;
        set => Set(ref logEnties, value);
    }

    public ICommand LoadedCommand { get; }

    public SettingsViewModel(ISettingsRepository seetingsRepository, MainWindowViewModel mainWindowViewModel, UserManager userManager)
    {
        this.seetingsRepository = seetingsRepository;
        this.mainWindowViewModel = mainWindowViewModel;
        this.userManager = userManager;
        LoadedCommand = new LambdaCommand(Loaded);
    }

    private void Loaded(object obj)
    {
        LogEntries = userManager.LogEntries;

        var allSettings = seetingsRepository.GetAll();

        IsTopmost = allSettings.ContainsKey(nameof(IsTopmost)) ? bool.Parse(allSettings[nameof(IsTopmost)]) : false;

        isLoaded = true;
    }
}
