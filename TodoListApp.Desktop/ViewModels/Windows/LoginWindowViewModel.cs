using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.IconPacks;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TodoListApp.Desktop.ViewModels.Windows;

public partial class LoginWindowViewModel : ObservableObject
{
    public event Action OnUserEnter;

    [ObservableProperty]
    private bool hasErrors = false;

    [ObservableProperty]
    private bool isConnecting = false;

    [ObservableProperty]
    private string userLogin = string.Empty;

    [NotifyPropertyChangedFor(nameof(SavePasswordIconState))]
    [ObservableProperty]
    private bool isSavePassword = true;

    public PackIconFontAwesomeKind SavePasswordIconState
    {
        get => IsSavePassword ? PackIconFontAwesomeKind.CheckSolid : PackIconFontAwesomeKind.StopSolid;
    }
    [RelayCommand]
    private async Task Login(PasswordBox pb)
    {

    }
}
