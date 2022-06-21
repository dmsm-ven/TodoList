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

    public ToolPanelViewModel()
    {
        MoveToTodoListCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.TodoList));
        MoveToShoppingListCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.ShoppingList));
        AddEmployeerCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.AddEmployeer));
        MoveToReadListCommand = new LambdaCommand(e => navigationLocator?.MoveTo(ViewModelType.ReadList));      
    }

    public ToolPanelViewModel(NavigationLocator navigationLocator) : this()
    {
        this.navigationLocator = navigationLocator;
        navigationLocator.ActiveViewModelChanged += () => RaisePropertyChanged(nameof(ActiveTab));
    }
}