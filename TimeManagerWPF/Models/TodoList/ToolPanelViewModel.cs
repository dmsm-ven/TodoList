using System.Windows.Input;

namespace TodoList.WPF.ViewModels;

public class ToolPanelViewModel : ViewModelBase
{
    private readonly NavigationLocator navigationLocator;

    public ViewModelType ActiveTab => navigationLocator?.ActiveViewModelType ?? ViewModelType.TodoList;
    public ICommand AddEmployeerCommand { get; }
    public ICommand MoveToTodoListCommand { get; }
    public ICommand MoveToShoppingListCommand { get; }
    public ICommand MoveToReadListCommand { get; }
    public ICommand MoveToSettingsViewCommand { get; }
    public ICommand MoveToBudgetViewCommand { get; }

    public ToolPanelViewModel()
    {
        MoveToTodoListCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.TodoList));
        MoveToShoppingListCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.ShoppingList));
        AddEmployeerCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.AddEmployeer));
        MoveToReadListCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.ReadList));
        MoveToSettingsViewCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.SettingsView));
        MoveToBudgetViewCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.BudgetView));
    }

    public ToolPanelViewModel(NavigationLocator navigationLocator) : this()
    {
        this.navigationLocator = navigationLocator;
        navigationLocator.ActiveViewModelChanged += () => RaisePropertyChanged(nameof(ActiveTab));
    }
}