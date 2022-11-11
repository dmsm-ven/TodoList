using System.Windows.Input;

namespace TodoList.WPF.ViewModels;

public class ConnectionErrorViewModel : ViewModelBase
{
    string errorMessage;
    public string ErrorMessage { get => errorMessage; set => Set(ref errorMessage, value); }

    public ICommand CloseApplicationCommand { get; }

    public ConnectionErrorViewModel()
    {
        CloseApplicationCommand = new LambdaCommand((o) => App.Current.Shutdown(), (o) => true);
    }
}
