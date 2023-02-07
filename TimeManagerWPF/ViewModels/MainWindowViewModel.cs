using System.Collections.Generic;
using System.Windows.Input;
using TodoList.WPF.DataAccess;
using TodoList.WPF.DataAccess.Repositories;

namespace TodoList.WPF.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    private readonly ISettingsRepository settings;

    public NavigationLocator NavigationLocator { get; }
    public ToolPanelViewModel ToolPanelViewModel { get; }

    bool isTopmost;
    public bool IsTopmost
    {
        get => isTopmost;
        set => Set(ref isTopmost, value);
    }

    public ICommand LoadedCommand { get; }

    public MainWindowViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
    }

    public MainWindowViewModel(ToolPanelViewModel toolPanelViewModel,
        NavigationLocator navigationLocator,
        ISettingsRepository settings) : this()
    {
        NavigationLocator = navigationLocator;
        ToolPanelViewModel = toolPanelViewModel;

        this.settings = settings;
    }

    private void Loaded(object obj)
    {
        NavigationLocator.MoveTo(ViewModelType.TodoList);

        var settingsDictionary = settings.GetAll();
        IsTopmost = settingsDictionary.ContainsKey(nameof(IsTopmost)) ?
            bool.Parse(settingsDictionary[nameof(IsTopmost)]) :
            false;
    }

}
