using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.Models;

namespace TodoList.WPF.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly UserManager userManager;

    IEnumerable<LogEntryEntity> logEnties;
    public IEnumerable<LogEntryEntity> LogEntries
    {
        get => logEnties;
        set => Set(ref logEnties, value);
    }

    public ICommand LoadedCommand { get; }

    public SettingsViewModel(UserManager userManager)
    {
        this.userManager = userManager;
        LoadedCommand = new LambdaCommand(Loaded);
    }

    private void Loaded(object obj)
    {
        LogEntries = userManager.LogEntries;
    }
}
