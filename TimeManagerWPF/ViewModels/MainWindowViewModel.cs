using System.Windows.Input;

namespace TodoList.WPF.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    public NavigationLocator NavigationLocator { get; }
    public ToolPanelViewModel ToolPanelViewModel { get; }
    
    public ICommand LoadedCommand { get; }

    public MainWindowViewModel()
    {
        LoadedCommand = new LambdaCommand(Loaded);
    }

    public MainWindowViewModel(ToolPanelViewModel toolPanelViewModel, NavigationLocator navigationLocator) : this()
    {
        NavigationLocator = navigationLocator; 
        ToolPanelViewModel = toolPanelViewModel;
    }

    private void Loaded(object obj)
    {
        NavigationLocator.MoveTo(ViewModelType.TodoList);
    }

}
