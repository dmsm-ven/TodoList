using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using SymbolRegular = Wpf.Ui.Controls.SymbolRegular;

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

    public SymbolRegular SavePasswordIconState
    {
        get => IsSavePassword ? SymbolRegular.CheckboxChecked24 : SymbolRegular.CheckboxUnchecked24;
    }
    [RelayCommand]
    private async Task Login(PasswordBox pb)
    {

    }
}
