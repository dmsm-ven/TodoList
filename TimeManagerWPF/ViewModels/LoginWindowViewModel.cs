using MahApps.Metro.IconPacks;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoList.WPF.DataAccess.Repositories;

namespace TodoList.WPF.ViewModels;


internal class LoginWindowViewModel : ViewModelBase
{
    private readonly IUserRepository userRepository;
    public event Action OnUserEnter;

    bool hasErrors;
    public bool HasErrors
    {
        get => hasErrors;
        private set => Set(ref hasErrors, value);
    }

    string login;
    public string Login
    {
        get => login;
        set => Set(ref login, value);
    }

    public bool isSavePassword = true;
    

    public bool IsSavePassword
    {
        get => isSavePassword;
        set 
        {
            if(Set(ref isSavePassword, value))
            {
                RaisePropertyChanged(nameof(SavePasswordIconState));
            }
        }
    }

    public ICommand LoginCommand { get; }

    public ICommand SavePasswordToggleCommand { get; }

    public PackIconFontAwesomeKind SavePasswordIconState
    {
        get => IsSavePassword ? PackIconFontAwesomeKind.CheckSolid : PackIconFontAwesomeKind.TimesSolid;
    }

    public LoginWindowViewModel()
    {
        SavePasswordToggleCommand = new LambdaCommand(e => IsSavePassword = !IsSavePassword);
        LoginCommand = new LambdaCommand(SignIn, e => !string.IsNullOrWhiteSpace(login));
    }

    public LoginWindowViewModel(IUserRepository userRepository) : this()
    {
        this.userRepository = userRepository;
    }

    private void SignIn(object o)
    {
        HasErrors = false;

        var password = (o as PasswordBox).Password;

        if(userRepository.Login(Login, password, IsSavePassword))
        {
            OnUserEnter?.Invoke();
        }
        else
        {
            HasErrors = true;
        }
    }
}
