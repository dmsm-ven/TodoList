namespace TodoList.WPF.ViewModels;

public class ConnectionErrorViewModel : ViewModelBase
{
    string errorMessage;
    public string ErrorMessage { get => errorMessage; set => Set(ref errorMessage, value); }
}
