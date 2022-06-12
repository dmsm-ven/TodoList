using System.Collections.ObjectModel;
using System.Linq;

namespace TodoList.WPF.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    public ObservableCollection<TodoTabViewModel> Tabs { get; set; }

    TodoTabViewModel selectedTab;
    public TodoTabViewModel SelectedTab
    {
        get => selectedTab;
        set => Set(ref selectedTab, value);
    }

    public TodoListViewModel()
    {
        Tabs = new ObservableCollection<TodoTabViewModel>()
        {
            new TodoTabViewModel(){ EmployeerName = "ЕТК-Комплект", HasActiveTask = true },
            new TodoTabViewModel(){ EmployeerName = "Виталий" },
            new TodoTabViewModel(){ EmployeerName = "Яна", HasActiveTask = true },
        };
        SelectedTab = Tabs.First();
    }
}
