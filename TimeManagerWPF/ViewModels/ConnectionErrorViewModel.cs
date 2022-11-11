using System.Windows.Input;

namespace TodoList.WPF.ViewModels;

public class ConnectionErrorWindowViewModel : ViewModelBase
{
    string errorMessage;
    public string ErrorMessage { get => errorMessage; set => Set(ref errorMessage, value); }

    public ICommand CloseApplicationCommand { get; }

    public ConnectionErrorWindowViewModel()
    {
        CloseApplicationCommand = new LambdaCommand((o) => App.Current.Shutdown(), (o) => true);
    }
}
