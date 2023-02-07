using MahApps.Metro.IconPacks;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoList.WPF.DataAccess.Repositories;
using TodoList.WPF.Models;

namespace TodoList.WPF.ViewModels;


internal class LoginWindowViewModel : ViewModelBase
{
    private readonly UserManager userManager;
    public event Action OnUserEnter;

    private bool hasErrors = false;
    public bool HasErrors
    {
        get => hasErrors;
        private set => Set(ref hasErrors, value);
    }

    private bool isConnecting = false;
    public bool IsConnecting
    {
        get => isConnecting;
        set => Set(ref isConnecting, value);
    }

    private string login;
    public string Login
    {
        get => login;
        set => Set(ref login, value);
    }

    public string DefaultPassword { get; set; } = string.Empty;

    public bool isSavePassword = true;

    public bool IsSavePassword
    {
        get => isSavePassword;
        set
        {
            if (Set(ref isSavePassword, value))
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
        LoginCommand = new LambdaCommand(async e => await SignIn(e), e => !string.IsNullOrWhiteSpace(login));
    }

    public LoginWindowViewModel(UserManager userManager) : this()
    {
        this.userManager = userManager;
    }

    private async Task SignIn(object o)
    {
        HasErrors = false;
        IsConnecting = true;

        string enteredPassword = (o as PasswordBox).Password;
        var password = !string.IsNullOrWhiteSpace(enteredPassword) ? enteredPassword : DefaultPassword;

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(1.25));
            var connectResult = await userManager.Login(Login, password);

            if (connectResult)
            {
                OnUserEnter?.Invoke();
            }
            else
            {
                HasErrors = true;
            }
        }
        catch (Exception ex)
        {
            HasErrors = true;
        }
        finally
        {
            IsConnecting = false;
        }

    }
}
